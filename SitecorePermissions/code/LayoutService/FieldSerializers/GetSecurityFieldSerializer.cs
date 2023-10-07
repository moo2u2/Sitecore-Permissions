using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;

namespace my.Foundation.Layout.LayoutService.FieldSerializers
{
    public class GetSecurityFieldSerializer : BaseGetFieldSerializer
    {
        public GetSecurityFieldSerializer(IFieldRenderer fieldRenderer) : base(fieldRenderer)
        {
        }

        protected override void SetResult(GetFieldSerializerPipelineArgs args)
        {
            args.Result = new SecurityFieldSerializer(FieldRenderer);
            args.AbortPipeline();
        }
    }
}