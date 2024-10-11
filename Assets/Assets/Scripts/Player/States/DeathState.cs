using KabreetGames.SceneManagement;
using KabreetGames.UiSystem;
using KendaAi.Agents.Planer;
using UnityEngine.SceneManagement;

namespace KendaAi.TestProject.PlayerController.States
{
    public class DeathState : State
    {
        public override async void Enter()
        {
            Agent.Animator.AnimationState.SetAnimation(0, GetAnimation(), true);
            var value = await MenuManager.DisplayModalWindowAsync("Game Over", "You lost!", null, "Restart",
                "MainMenu");
            switch (value)
            {
                case 0:
                    await SceneLoadingManager.Restart(loadSceneMode : LoadSceneMode.Single);
                    break;
                case 1:
                    await SceneLoadingManager.ReplaceScene(SceneGroupNames.MainMenu, SceneGroupNames.GamePlay, loadSceneMode: LoadSceneMode.Single);
                    break;
            }
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void Exit()
        {
        }
    }
}