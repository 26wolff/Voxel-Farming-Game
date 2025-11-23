using ComputeSharp;

namespace Render
{
    [ThreadGroupSize(64,1,1)]
    [GeneratedComputeShaderDescriptor] // Required
    public partial struct RenderKernel : IComputeShader
    {
        public ReadWriteBuffer<int> Buffer;

        public void Execute()
        {
            
        }
    }
}
