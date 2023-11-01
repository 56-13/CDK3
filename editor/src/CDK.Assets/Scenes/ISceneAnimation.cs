
namespace CDK.Assets.Scenes
{
    public interface ISceneAnimation
    {
        float Progress { get; }
        float GetDuration(DurationParam param);
    }
}
