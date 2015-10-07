using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AppFramework.Core.Classes.SearchEngine.RPN
{
    public class SubsequentQueryCalculator
    {
        protected Opus6.Stack _stack;
        protected SubsequentQueryConverter _converter;

        /// <summary>
        /// Class constructor
        /// </summary>
        public SubsequentQueryCalculator()
        {
            _stack = new Opus6.StackAsLinkedList();
            _converter = new SubsequentQueryConverter();
        }

        /// <summary>
        /// Calculates the result of subqueries and returns the collection of assets
        /// </summary>
        /// <param name="subqueries"></param>
        /// <returns></returns>
        public List<Asset> Calculate(List<SubsequentQuery> subqueries)
        {
            _stack.Purge();
            string rpn = _converter.ToRpn(subqueries);

            List<AttributeElement> elements = (from s in subqueries
                                               select s.AttributeElement).ToList();

            foreach (string part in rpn.Split(new char[] { ' ' }).Where(s => !string.IsNullOrEmpty(s)))
            {
                //if (part.Length > 1)
                //{
                //    List<Asset> chainResult = subqueries.Single(s => s.SearchChain.GetHashCode() == Convert.ToInt32(part)).Result;
                //    _stack.Push(chainResult);
                //}
                //else if (_converter.IsArithmeticOperation(Convert.ToChar(part)))
                //{
                //    List<Asset> op1;
                //    List<Asset> op2;
                //    try
                //    {
                //        op1 = _stack.Pop() as List<Asset>;
                //    }
                //    catch (Opus6.ContainerEmptyException ex)
                //    {
                //        op1 = new List<Asset>();
                //    }
                //    try
                //    {
                //        op2 = _stack.Pop() as List<Asset>;
                //    }
                //    catch (Opus6.ContainerEmptyException ex)
                //    {
                //        op2 = new List<Asset>();
                //    }
                //    _stack.Push(DoOperation(op2, op1, part));
                //}
                //else
                //{
                //    throw new InvalidOperationException("Unknown stack content: " + part);
                //}
            }
            return _stack.Pop() as List<Asset>;
        }

        protected List<Asset> DoOperation(List<Asset> op1, List<Asset> op2, string operation)
        {
            switch (operation)
            {
                case "+": return op1.Union(op2, new AssetsComparer()).ToList();
                case "*": return op1.Intersect(op2, new AssetsComparer()).ToList();
                default: throw new InvalidOperationException();
            }
        }
    }
}
