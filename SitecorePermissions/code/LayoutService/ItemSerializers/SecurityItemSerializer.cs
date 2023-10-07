using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization.ItemSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;

namespace my.Foundation.Layout.LayoutService.ItemSerializers
{
    public class SecurityItemSerializer : DefaultItemSerializer
    {
        public SecurityItemSerializer(IGetFieldSerializerPipeline getFieldSerializerPipeline) : base(getFieldSerializerPipeline)
        {
        }

        // Allow the security field from standard fields
        protected override bool FieldFilter(Field field)
        {
            Assert.ArgumentNotNull(field, nameof(field));
            return field.Name == "__Security" || !field.Name.StartsWith("__", System.StringComparison.Ordinal);
        }
    }
}