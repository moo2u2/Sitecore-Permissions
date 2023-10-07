using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.JavaScriptServices.ViewEngine.LayoutService;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;

namespace my.Foundation.Layout.LayoutService.ItemSerializers
{
    public class SecurityJssItemSerializer : JssItemSerializer
    {
        public SecurityJssItemSerializer(IGetFieldSerializerPipeline getFieldSerializerPipeline) : base(getFieldSerializerPipeline)
        {
        }

        // Allow the security field from standard fields, don't serialise "Allowed Renderings" from SXA Styles
        protected override bool FieldFilter(Field field)
        {
            Assert.ArgumentNotNull(field, nameof(field));
            return (field.Name == "__Security" || !field.Name.StartsWith("__", System.StringComparison.Ordinal)) && field.ID != Constants.AllowedRenderingsFieldId;
        }
    }
}