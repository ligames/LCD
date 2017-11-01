using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GlobalInstaller", menuName = "Installers/GlobalInstaller")]
public class GlobalInstaller : ScriptableObjectInstaller<GlobalInstaller>
{
    [SerializeField]
    GlobalInfo globalInfo;
    [SerializeField]
    StateDefaultInfo stateDefaultInfo;
    [SerializeField]
    FieldInfo fieldInfo;
    [SerializeField]
    CameraMovementConfig cameraMotionConfig;
    public override void InstallBindings()
    {
        Container.Bind<GlobalInfo>().To<GlobalInfo>().FromInstance(globalInfo).AsSingle();
        Container.Bind<FieldInfo>().To<FieldInfo>().FromInstance(fieldInfo).AsSingle();
        Container.Bind<CameraMovementConfig>().FromInstance(cameraMotionConfig).AsSingle();
        Container.Bind<StateDefaultInfo>().
            To<StateDefaultInfo>().
            FromInstance(stateDefaultInfo).
            AsSingle();
    }
}