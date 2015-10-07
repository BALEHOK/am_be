namespace AppFramework.DataProxy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects.DataClasses;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization;

    /// <summary>
    /// Entity Cloner from Matthieu MEZIL
    /// http://msmvps.com/blogs/matthieu/archive/2008/05/31/entity-cloner.aspx
    /// </summary>
    public static class EntityObjectExtension
    {
        //public static T Clone<T>(this T entityObject) where T : EntityObject, new()
        //{
        //    return EntityCloner<T>.Clone(entityObject);
        //}

        //public static T CloneWithGraph<T>(this T entityObject) where T : EntityObject, new()
        //{
        //    return EntityCloner<T>.CloneWithGraph(entityObject);
        //}

        public static T Clone<T>(this T source) where T : AppFramework.Entities.IDataEntity, new()
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                dcs.WriteObject(ms, source);
                ms.Position = 0;
                return (T)dcs.ReadObject(ms);
            }
        }
    }

    public static class EntityCloner<T> where T : EntityObject, new()
    {
        private static Func<T, T> _cloneDelegate;
        private static Func<T, List<EntityObject>, T> _cloneWithGraphDelegate;
        private static Func<PropertyInfo, bool> _clonePropertyDelegateStep1;
        private static Func<PropertyInfo, bool> _clonePropertyDelegateStep2;

        private static Func<T, T> CloneDelegate
        {
            get
            {
                if (_cloneDelegate == null)
                {

                    var generatedMethod = GenerateCloneMethod(ClonePropertyDelegateStep1, null).CreateDelegate(typeof(Func<T, List<EntityObject>, Func<EntityObject, List<EntityObject>, EntityObject>, T>)) as Func<T, List<EntityObject>, Func<EntityObject, List<EntityObject>, EntityObject>, T>;

                    _cloneDelegate = entity => generatedMethod(entity, new List<EntityObject>(), Cloner.CloneEntity);

                }

                return _cloneDelegate;

            }

        }

        private static Func<T, List<EntityObject>, T> CloneWithGraphDelegate
        {

            get
            {

                if (_cloneWithGraphDelegate == null)
                {

                    var generatedMethod = GenerateCloneMethod(ClonePropertyDelegateStep1, ClonePropertyDelegateStep2).CreateDelegate(typeof(Func<T, List<EntityObject>, Func<EntityObject, List<EntityObject>, EntityObject>, T>)) as Func<T, List<EntityObject>, Func<EntityObject, List<EntityObject>, EntityObject>, T>;

                    _cloneWithGraphDelegate = (entity, entitiesAlreadyCloned) => generatedMethod(entity, entitiesAlreadyCloned, Cloner.CloneEntity);

                }

                return _cloneWithGraphDelegate;

            }

        }

        private static bool AllowClonePropertyStep1(PropertyInfo pi)
        {

            return !(typeof(EntityObject).IsAssignableFrom(pi.PropertyType) ||

                pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>));

        }

        public static Func<PropertyInfo, bool> ClonePropertyDelegateStep1
        {

            get
            {

                if (_clonePropertyDelegateStep1 == null)

                    return pi => AllowClonePropertyStep1(pi);

                return _clonePropertyDelegateStep1;

            }

            set
            {

                if (_clonePropertyDelegateStep1 != value)
                {

                    _clonePropertyDelegateStep1 = value;

                    _cloneDelegate = null;

                }

            }

        }

        public static Func<PropertyInfo, bool> ClonePropertyDelegateStep2
        {

            get
            {

                if (_clonePropertyDelegateStep2 == null)

                    return pi => !AllowClonePropertyStep1(pi);

                return _clonePropertyDelegateStep2;

            }

            set
            {

                if (_clonePropertyDelegateStep2 != value)
                {

                    _clonePropertyDelegateStep2 = value;

                    _cloneDelegate = null;

                }

            }

        }



        private static DynamicMethod GenerateCloneMethod(Func<PropertyInfo, bool> step1, Func<PropertyInfo, bool> step2)
        {

            var dynamicMethod = new DynamicMethod("Clone", typeof(T), new Type[] { typeof(T), typeof(List<EntityObject>), typeof(Func<EntityObject, List<EntityObject>, EntityObject>) });

            var cloneIlGenerator = dynamicMethod.GetILGenerator();

            cloneIlGenerator.Emit(OpCodes.Ldarg_0);

            var argNotNullLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Brtrue_S, argNotNullLabel);

            cloneIlGenerator.Emit(OpCodes.Ldnull);

            cloneIlGenerator.Emit(OpCodes.Ret);

            cloneIlGenerator.MarkLabel(argNotNullLabel);

            CopyObject(step1, step2, cloneIlGenerator, typeof(T), () => cloneIlGenerator.Emit(OpCodes.Ldarg_0), () => cloneIlGenerator.Emit(OpCodes.Ldarg_1), () => cloneIlGenerator.Emit(OpCodes.Ldarg_2));

            cloneIlGenerator.Emit(OpCodes.Ret);

            return dynamicMethod;

        }



        private static void CopyObject(Func<PropertyInfo, bool> step1, Func<PropertyInfo, bool> step2, ILGenerator cloneIlGenerator, Type typeT, Action getSource, Action getEntitiesList, Action getCloneLinkedEntities)
        {

            EntityAlreadyCloned(cloneIlGenerator, getEntitiesList, getSource);

            cloneIlGenerator.Emit(OpCodes.Castclass, typeT);

            var value = cloneIlGenerator.DeclareLocal(typeT);

            cloneIlGenerator.Emit(OpCodes.Stloc_S, value);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, value);

            var endExistingConditionMethodLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Brtrue, endExistingConditionMethodLabel);

            cloneIlGenerator.Emit(OpCodes.Newobj, typeT.GetConstructor(new Type[0]));

            cloneIlGenerator.Emit(OpCodes.Stloc_S, value);

            CopyProps(step1, step2, cloneIlGenerator, value, typeT, getSource, getEntitiesList, getCloneLinkedEntities);

            cloneIlGenerator.MarkLabel(endExistingConditionMethodLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc, value);

        }



        private static void CopyProps(Func<PropertyInfo, bool> step1, Func<PropertyInfo, bool> step2, ILGenerator cloneIlGenerator, LocalBuilder value, Type typeT, Action getSource, Action getEntitiesList, Action getCloneLinkedEntities)
        {

            foreach (var prop in typeT.GetProperties().Where(p => p.CanRead && p.CanWrite && step1 != null && step1(p)))

                ClonePerStep(step1, step2, cloneIlGenerator, value, typeT, getSource, getEntitiesList, getCloneLinkedEntities, prop);

            getEntitiesList();

            cloneIlGenerator.Emit(OpCodes.Ldloc, value);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(List<EntityObject>).GetMethod("Add"));

            foreach (var prop in typeT.GetProperties().Where(p => p.CanRead && p.CanWrite && step2 != null && step2(p)))

                ClonePerStep(step1, step2, cloneIlGenerator, value, typeT, getSource, getEntitiesList, getCloneLinkedEntities, prop);

        }



        private static void ClonePerStep(Func<PropertyInfo, bool> step1, Func<PropertyInfo, bool> step2, ILGenerator cloneIlGenerator, LocalBuilder value, Type typeT, Action getSource, Action getEntitiesList, Action getCloneLinkedEntities, PropertyInfo prop)
        {

            if (typeof(ComplexObject).IsAssignableFrom(prop.PropertyType))
            {

                ConstructorInfo complexObjectCtor = prop.PropertyType.GetConstructor(new Type[0]);

                if (complexObjectCtor != null)
                {

                    cloneIlGenerator.Emit(OpCodes.Ldloc, value);

                    var sourceComplexTypeProp = cloneIlGenerator.DeclareLocal(prop.PropertyType);

                    var valueComplexTypeProp = cloneIlGenerator.DeclareLocal(prop.PropertyType);

                    getSource();

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("get_" + prop.Name));

                    cloneIlGenerator.Emit(OpCodes.Stloc_S, sourceComplexTypeProp);

                    cloneIlGenerator.Emit(OpCodes.Newobj, complexObjectCtor);

                    cloneIlGenerator.Emit(OpCodes.Stloc_S, valueComplexTypeProp);

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, valueComplexTypeProp);

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("set_" + prop.Name));

                    CopyProps(step1, step2, cloneIlGenerator, valueComplexTypeProp, prop.PropertyType, () => cloneIlGenerator.Emit(OpCodes.Ldloc, sourceComplexTypeProp), getEntitiesList, getCloneLinkedEntities);

                }

            }

            else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>))

                CopyEntityReference(cloneIlGenerator, () => cloneIlGenerator.Emit(OpCodes.Ldloc, value), typeT, getSource, prop, false);

            else if (typeof(EntityObject).IsAssignableFrom(prop.PropertyType))
            {

                if (prop.PropertyType.GetConstructor(new Type[0]) != null)
                {

                    var entityObjectProp = cloneIlGenerator.DeclareLocal(prop.PropertyType);

                    getSource();

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("get_" + prop.Name));

                    cloneIlGenerator.Emit(OpCodes.Stloc_S, entityObjectProp);

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityObjectProp);

                    var entityObjectNullLabel = cloneIlGenerator.DefineLabel();

                    cloneIlGenerator.Emit(OpCodes.Brfalse_S, entityObjectNullLabel);

                    cloneIlGenerator.Emit(OpCodes.Ldloc, value);

                    getCloneLinkedEntities();

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityObjectProp);

                    getEntitiesList();

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(Func<EntityObject, List<EntityObject>, EntityObject>).GetMethod("Invoke"));

                    cloneIlGenerator.Emit(OpCodes.Castclass, prop.PropertyType);

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("set_" + prop.Name));

                    cloneIlGenerator.MarkLabel(entityObjectNullLabel);

                }

            }

            else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>))
            {

                var entityCollectionSourceProp = cloneIlGenerator.DeclareLocal(prop.PropertyType);

                getSource();

                cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("get_" + prop.Name));

                cloneIlGenerator.Emit(OpCodes.Stloc_S, entityCollectionSourceProp);

                cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityCollectionSourceProp);

                var entityCollectionNullLabel = cloneIlGenerator.DefineLabel();

                cloneIlGenerator.Emit(OpCodes.Brfalse_S, entityCollectionNullLabel);

                cloneIlGenerator.Emit(OpCodes.Ldloc, value);

                var entityCollectionValueProp = cloneIlGenerator.DeclareLocal(prop.PropertyType);

                cloneIlGenerator.Emit(OpCodes.Newobj, prop.PropertyType.GetConstructor(new Type[0]));

                Type subEntityType = prop.PropertyType.GetGenericArguments()[0];

                if (subEntityType.GetConstructor(new Type[0]) != null)
                {

                    cloneIlGenerator.Emit(OpCodes.Stloc_S, entityCollectionValueProp);

                    var entityCollectionSourceEnumerator = cloneIlGenerator.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(subEntityType));

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityCollectionSourceProp);

                    cloneIlGenerator.Emit(OpCodes.Callvirt, prop.PropertyType.GetMethod("GetEnumerator"));

                    cloneIlGenerator.Emit(OpCodes.Stloc_S, entityCollectionSourceEnumerator);

                    var startAddEntityCollectionLabel = cloneIlGenerator.DefineLabel();

                    var endAddEntityCollectionLabel = cloneIlGenerator.DefineLabel();

                    cloneIlGenerator.MarkLabel(startAddEntityCollectionLabel);

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityCollectionSourceEnumerator);

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(IEnumerator).GetMethod("MoveNext"));

                    cloneIlGenerator.Emit(OpCodes.Brfalse_S, endAddEntityCollectionLabel);

                    var subEntityObject = cloneIlGenerator.DeclareLocal(subEntityType);

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityCollectionSourceEnumerator);

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(IEnumerator<>).MakeGenericType(subEntityType).GetMethod("get_Current"));

                    cloneIlGenerator.Emit(OpCodes.Stloc_S, subEntityObject);

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityCollectionValueProp);

                    getCloneLinkedEntities();

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, subEntityObject);

                    getEntitiesList();

                    cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(Func<EntityObject, List<EntityObject>, EntityObject>).GetMethod("Invoke"));

                    cloneIlGenerator.Emit(OpCodes.Castclass, subEntityType);

                    cloneIlGenerator.Emit(OpCodes.Callvirt, prop.PropertyType.GetMethod("Add", new Type[] { subEntityType }));

                    cloneIlGenerator.Emit(OpCodes.Br_S, startAddEntityCollectionLabel);

                    cloneIlGenerator.MarkLabel(endAddEntityCollectionLabel);

                    cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityCollectionValueProp);

                }

                cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("set_" + prop.Name));

                cloneIlGenerator.MarkLabel(entityCollectionNullLabel);

            }

            else
            {

                cloneIlGenerator.Emit(OpCodes.Ldloc, value);

                getSource();

                cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("get_" + prop.Name));

                cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("set_" + prop.Name));

            }

        }



        internal static void CopyEntityReference(ILGenerator cloneIlGenerator, Action loadNewObject, Type typeT, Action getSource, PropertyInfo prop, bool copyNull)
        {

            var sourceEntityReferenceEntityKeyProp = cloneIlGenerator.DeclareLocal(typeof(EntityKey));

            var sourceEntityReferenceEntityKeyMemberProp = cloneIlGenerator.DeclareLocal(typeof(EntityKeyMember));

            var sourceEntityReferenceEntityKeysMemberProp = cloneIlGenerator.DeclareLocal(typeof(EntityKeyMember[]));

            var sourceEntityReferenceEntityKeysMemberLength = cloneIlGenerator.DeclareLocal(typeof(int));

            var valueEntityReferenceProp = cloneIlGenerator.DeclareLocal(prop.PropertyType);

            var valueEntityReferenceEntityKeyProp = cloneIlGenerator.DeclareLocal(typeof(EntityKey));

            var valueEntityReferenceEntityKeysMemberProp = cloneIlGenerator.DeclareLocal(typeof(EntityKeyMember[]));

            var loopIndex = cloneIlGenerator.DeclareLocal(typeof(int));

            loadNewObject();

            cloneIlGenerator.Emit(OpCodes.Newobj, prop.PropertyType.GetConstructor(new Type[0]));

            cloneIlGenerator.Emit(OpCodes.Stloc_S, valueEntityReferenceProp);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, valueEntityReferenceProp);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("set_" + prop.Name));

            getSource();

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeT.GetMethod("get_" + prop.Name));

            cloneIlGenerator.Emit(OpCodes.Callvirt, prop.PropertyType.GetMethod("get_EntityKey"));

            cloneIlGenerator.Emit(OpCodes.Stloc_S, sourceEntityReferenceEntityKeyProp);

            var entityKeyNullLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeyProp);

            cloneIlGenerator.Emit(OpCodes.Brfalse_S, entityKeyNullLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, valueEntityReferenceProp);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeyProp);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityKey).GetMethod("get_EntityKeyValues")); // We suppose EntityKeyValues is not null

            cloneIlGenerator.Emit(OpCodes.Stloc_S, sourceEntityReferenceEntityKeysMemberProp);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeysMemberProp);

            cloneIlGenerator.Emit(OpCodes.Ldlen);

            cloneIlGenerator.Emit(OpCodes.Conv_I4);

            cloneIlGenerator.Emit(OpCodes.Stloc_S, sourceEntityReferenceEntityKeysMemberLength);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeysMemberLength);

            cloneIlGenerator.Emit(OpCodes.Newarr, typeof(EntityKeyMember));

            cloneIlGenerator.Emit(OpCodes.Stloc_S, valueEntityReferenceEntityKeysMemberProp);

            var noEntityKeyValues = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeysMemberLength);

            cloneIlGenerator.Emit(OpCodes.Brfalse_S, noEntityKeyValues);

            cloneIlGenerator.Emit(OpCodes.Ldc_I4_0);

            cloneIlGenerator.Emit(OpCodes.Stloc_S, loopIndex);

            var startLoopLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.MarkLabel(startLoopLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeysMemberLength);

            cloneIlGenerator.Emit(OpCodes.Ceq);

            var endOfLoopLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Brtrue_S, endOfLoopLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeysMemberProp);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Ldelem_Ref);

            cloneIlGenerator.Emit(OpCodes.Stloc_S, sourceEntityReferenceEntityKeyMemberProp);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, valueEntityReferenceEntityKeysMemberProp);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeyMemberProp);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityKeyMember).GetMethod("get_Key"));

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeyMemberProp);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityKeyMember).GetMethod("get_Value"));

            cloneIlGenerator.Emit(OpCodes.Newobj, typeof(EntityKeyMember).GetConstructor(new Type[] { typeof(string), typeof(object) }));

            cloneIlGenerator.Emit(OpCodes.Stelem_Ref);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Ldc_I4_1);

            cloneIlGenerator.Emit(OpCodes.Add);

            cloneIlGenerator.Emit(OpCodes.Stloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Br_S, startLoopLabel);

            cloneIlGenerator.MarkLabel(endOfLoopLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeyProp);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityKey).GetMethod("get_EntityContainerName"));

            cloneIlGenerator.Emit(OpCodes.Ldstr, ".");

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, sourceEntityReferenceEntityKeyProp);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityKey).GetMethod("get_EntitySetName"));

            cloneIlGenerator.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string) })); // We suppose EntityContainerName and EntitySetName aren't null

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, valueEntityReferenceEntityKeysMemberProp);

            cloneIlGenerator.Emit(OpCodes.Newobj, typeof(EntityKey).GetConstructor(new Type[] { typeof(string), typeof(IEnumerable<EntityKeyMember>) }));

            var noEntityKeyValuesEnd = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Br, noEntityKeyValuesEnd);

            cloneIlGenerator.MarkLabel(noEntityKeyValues);

            cloneIlGenerator.Emit(OpCodes.Newobj, typeof(EntityKey).GetConstructor(new Type[0]));

            cloneIlGenerator.MarkLabel(noEntityKeyValuesEnd);

            cloneIlGenerator.Emit(OpCodes.Callvirt, prop.PropertyType.GetMethod("set_EntityKey"));

            if (copyNull)
            {

                var endCopyReferenceLabel = cloneIlGenerator.DefineLabel();

                cloneIlGenerator.Emit(OpCodes.Br_S, endCopyReferenceLabel);

                cloneIlGenerator.MarkLabel(entityKeyNullLabel);

                cloneIlGenerator.Emit(OpCodes.Ldloc_S, valueEntityReferenceProp);

                cloneIlGenerator.Emit(OpCodes.Ldnull);

                cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(T).GetMethod("set_" + prop.Name));

                cloneIlGenerator.MarkLabel(endCopyReferenceLabel);

            }

            else

                cloneIlGenerator.MarkLabel(entityKeyNullLabel);

        }



        private static void EntityAlreadyCloned(ILGenerator cloneIlGenerator, Action getEntitiesList, Action getSource)
        {

            var loopIndex = cloneIlGenerator.DeclareLocal(typeof(int));

            var entitiesListCount = cloneIlGenerator.DeclareLocal(typeof(int));

            cloneIlGenerator.Emit(OpCodes.Ldc_I4_0);

            cloneIlGenerator.Emit(OpCodes.Stloc_S, loopIndex);

            getEntitiesList();

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(List<EntityObject>).GetMethod("get_Count"));

            cloneIlGenerator.Emit(OpCodes.Stloc_S, entitiesListCount);

            var startLoopLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.MarkLabel(startLoopLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, entitiesListCount);

            cloneIlGenerator.Emit(OpCodes.Ceq);

            var endLoopLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Brtrue_S, endLoopLabel);

            var endLabel = cloneIlGenerator.DefineLabel();

            getEntitiesList();

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(List<EntityObject>).GetMethod("get_Item"));

            var entityAlreadyCloned = cloneIlGenerator.DeclareLocal(typeof(EntityObject));

            cloneIlGenerator.Emit(OpCodes.Stloc_S, entityAlreadyCloned);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityAlreadyCloned);

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityObject).GetMethod("get_EntityKey"));

            getSource();

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityObject).GetMethod("get_EntityKey"));

            cloneIlGenerator.Emit(OpCodes.Callvirt, typeof(EntityKey).GetMethod("Equals", new Type[] { typeof(EntityKey) }));

            var entityKeysDifferentsLabel = cloneIlGenerator.DefineLabel();

            cloneIlGenerator.Emit(OpCodes.Brfalse_S, entityKeysDifferentsLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, entityAlreadyCloned);

            cloneIlGenerator.Emit(OpCodes.Br_S, endLabel);

            cloneIlGenerator.MarkLabel(entityKeysDifferentsLabel);

            cloneIlGenerator.Emit(OpCodes.Ldloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Ldc_I4_1);

            cloneIlGenerator.Emit(OpCodes.Add);

            cloneIlGenerator.Emit(OpCodes.Stloc_S, loopIndex);

            cloneIlGenerator.Emit(OpCodes.Br_S, startLoopLabel);

            cloneIlGenerator.MarkLabel(endLoopLabel);

            cloneIlGenerator.Emit(OpCodes.Ldnull);

            cloneIlGenerator.MarkLabel(endLabel);

        }



        public static T Clone(T obj)
        {

            return CloneDelegate(obj);

        }

        public static T CloneWithGraph(T obj)
        {

            return CloneWithGraph(obj, new List<EntityObject>());

        }

        internal static T CloneWithGraph(T obj, List<EntityObject> entitiesAlreadyCloned)
        {

            return CloneWithGraphDelegate(obj, entitiesAlreadyCloned);

        }

    }



    internal class Cloner
    {

        private static Dictionary<Type, Delegate> _cached = new Dictionary<Type, Delegate>();



        internal static EntityObject CloneEntity(EntityObject entity, List<EntityObject> entitiesAlreadyCloned)
        {

            Type entityType = entity.GetType();

            if (!_cached.ContainsKey(entityType))
            {

                ParameterExpression paramE = Expression.Parameter(typeof(EntityObject), "e");

                ParameterExpression paramL = Expression.Parameter(typeof(List<EntityObject>), "l");

                Expression<Func<EntityObject, List<EntityObject>, EntityObject>> generatedExpression =

                    Expression.Lambda<Func<EntityObject, List<EntityObject>, EntityObject>>(

                        Expression.Call(typeof(EntityCloner<>).MakeGenericType(entityType).GetMethod("CloneWithGraph", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { entityType, typeof(List<EntityObject>) }, null),

                            Expression.Convert(

                                paramE,

                                entityType),

                            paramL),

                        paramE,

                        paramL);



                _cached.Add(entityType, generatedExpression.Compile());

            }

            return _cached[entityType].DynamicInvoke(entity, entitiesAlreadyCloned) as EntityObject;

        }
    }
}
