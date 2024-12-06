using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthNormalInputFeature : ScriptableRendererFeature
{
    public class DepthNormalPass : ScriptableRenderPass
    {
        public DepthNormalPass()
        {
            // Настраиваем событие выполнения Pass (можно выбрать любое подходящее)
            renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Указываем системе, что нам нужны глубина и нормали
            ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // В данном Pass ничего рендерить не нужно
        }
    }

    private DepthNormalPass _depthNormalPass;

    public override void Create()
    {
        // Создаём экземпляр Pass
        _depthNormalPass = new DepthNormalPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Добавляем Pass в очередь
        renderer.EnqueuePass(_depthNormalPass);
    }
}