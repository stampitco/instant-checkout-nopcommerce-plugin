using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.InstantCheckout.Attributes;
using Nop.Plugin.Widgets.InstantCheckout.Constants;

namespace Nop.Plugin.Widgets.InstantCheckout.Maps
{
    public class JsonPropertyMapper : IJsonPropertyMapper
    {
        private IStaticCacheManager _cacheManager;

        private IStaticCacheManager StaticCacheManager
        {
            get
            {
                if (_cacheManager == null)
                {
                    _cacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
                }

                return _cacheManager;
            }
        }

        public Dictionary<string, Tuple<string, Type>> GetMap(Type type)
        {
            if (!StaticCacheManager.IsSet(Configurations.JSON_TYPE_MAPS_PATTERN))
            {
                StaticCacheManager.Set(Configurations.JSON_TYPE_MAPS_PATTERN, new Dictionary<string, Dictionary<string, Tuple<string, Type>>>(), int.MaxValue);
            }

            var typeMaps = StaticCacheManager.Get<Dictionary<string, Dictionary<string, Tuple<string, Type>>>>(Configurations.JSON_TYPE_MAPS_PATTERN, () => null);

            if (!typeMaps.ContainsKey(type.Name))
            {
                Build(type);
            }

            return typeMaps[type.Name];
        }

        private void Build(Type type)
        {
            var typeMaps =
                StaticCacheManager.Get<Dictionary<string, Dictionary<string, Tuple<string, Type>>>>(Configurations.JSON_TYPE_MAPS_PATTERN, () => null);

            var mapForCurrentType = new Dictionary<string, Tuple<string, Type>>();

            var typeProps = type.GetProperties();

            foreach (var property in typeProps)
            {
                var jsonAttribute = property.GetCustomAttribute(typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;
                var doNotMapAttribute = property.GetCustomAttribute(typeof(DoNotMapAttribute)) as DoNotMapAttribute;

                // If it has json attribute set and is not marked as doNotMap
                if (jsonAttribute != null && doNotMapAttribute == null)
                {
                    if (!mapForCurrentType.ContainsKey(jsonAttribute.PropertyName))
                    {
                        var value = new Tuple<string, Type>(property.Name, property.PropertyType);
                        mapForCurrentType.Add(jsonAttribute.PropertyName, value);
                    }
                }
            }

            if (!typeMaps.ContainsKey(type.Name))
            {
                typeMaps.Add(type.Name, mapForCurrentType);
            }
        }
    }
}