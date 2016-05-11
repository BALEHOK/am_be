using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetSite
{
    public class AutoMapperConfig
    {
        public static void RegisterDataMappings()
        {
            AutoMapper.Mapper.CreateMap<AppFramework.Entities.DynEntityConfig, AppFramework.Entities.DynEntityConfig>()
                .ForMember(dec => dec.AttributePanel, opt => opt.Ignore())
                .ForMember(dec => dec.AssetTypeScreen, opt => opt.Ignore())
                .ForMember(dec => dec.ChangeTracker, opt => opt.Ignore())
                .ForMember(dec => dec.DynListValue, opt => opt.Ignore())
                .ForMember(dec => dec.DynEntityAttribConfigs, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<AppFramework.Entities.DynEntityAttribConfig, AppFramework.Entities.DynEntityAttribConfig>()
                .ForMember(deac => deac.ChangeTracker, opt => opt.Ignore())
                .ForMember(dec => dec.AttributePanelAttributes, opt => opt.Ignore())
                .ForMember(deac => deac.DataType, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<AppFramework.Entities.AttributePanel, AppFramework.Entities.AttributePanel>()
                .ForMember(ap => ap.AttributePanelAttribute, opt => opt.Ignore())
                .ForMember(ap => ap.ChangeTracker, opt => opt.Ignore())
                .ForMember(ap => ap.AssetTypeScreen, opt => opt.Ignore());
            AutoMapper.Mapper.CreateMap<AppFramework.Entities.AssetTypeScreen, AppFramework.Entities.AssetTypeScreen>()
                .ForMember(s => s.ChangeTracker, opt => opt.Ignore())
                .ForMember(s => s.DynEntityConfig, opt => opt.Ignore())
                .ForMember(s => s.AttributePanel, opt => opt.Ignore());
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }
    }
}