using Sitecore.Data;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.ItemSerializers;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;

namespace my.Foundation.Layout.Pipelines.Initialize
{
    public class SerializeRenderingParameters : Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering.Initialize
    {
        IRenderingConfiguration _renderingConfiguration;

        public SerializeRenderingParameters(IConfiguration configuration) : base(configuration) { }

        protected override RenderedJsonRendering CreateResultInstance(RenderJsonRenderingArgs args)
        {
            string componentName = GetComponentName(args.Rendering?.RenderingItem?.InnerItem);
            _renderingConfiguration = args.RenderingConfiguration;

            return new RenderedJsonRendering()
            {
                ComponentName = (componentName ?? args.Rendering.RenderingItem.Name),
                DataSource = args.Rendering.DataSource,
                RenderingParams = SerializeRenderingParams(args.Rendering),
                Uid = args.Rendering.UniqueId
            };
        }

        protected virtual IDictionary<string, string> SerializeRenderingParams(Rendering rendering)
        {
            return SerializeParams(_renderingConfiguration.ItemSerializer, rendering.Parameters, rendering.RenderingItem.Database);   
        }

        public static IDictionary<string,string> SerializeParams(IItemSerializer itemSerializer, RenderingParameters parameters, Database database)
        {
            IDictionary<string, string> paramDictionary = parameters.ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (string key in paramDictionary.Keys.ToList())
            {
                if (paramDictionary[key].Contains('|'))
                {
                    string[] ids = paramDictionary[key].Split('|');
                    paramDictionary[key] = "[";
                    foreach (string id in ids)
                    {
                        if (ID.TryParse(id, out var itemId))
                        {
                            if (paramDictionary[key] != "[") paramDictionary[key] += ",";
                            Sitecore.Data.Items.Item paramItem = database.GetItem(itemId);
                            if (paramItem == null)
                                Sitecore.Diagnostics.Log.Error($"Could not find item in rendering params with ID {itemId}", typeof(SerializeRenderingParameters));
                            else if(paramItem.TemplateID != Constants.JsonRenderingTemplateId)
                                paramDictionary[key] += itemSerializer.Serialize(paramItem, new SerializationOptions() { DisableEditing = true });
                        }
                    }
                    paramDictionary[key] += "]";
                }
                else if (ID.TryParse(paramDictionary[key], out var itemId))
                {
                    Sitecore.Data.Items.Item paramItem = database.GetItem(itemId);
                    if (paramItem == null)
                        Sitecore.Diagnostics.Log.Error($"Could not find item in rendering params with ID {itemId}", typeof(SerializeRenderingParameters));
                    else if (paramItem.TemplateID != Constants.JsonRenderingTemplateId)
                        paramDictionary[key] = itemSerializer.Serialize(paramItem, new SerializationOptions() { DisableEditing = true });
                }
            }
            return paramDictionary;
        }
    }
}