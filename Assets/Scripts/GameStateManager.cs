public class GameStateManager
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameStateManager();

            return _instance;
        }
    }

    public bool paused;

    public delegate void GameStateChangeHandler(bool newGameState);
    public event GameStateChangeHandler OnGameStateChanged;

    public void SetState(bool newGameState)
    {
        if (newGameState == paused)
            return;

        paused = newGameState;
        OnGameStateChanged?.Invoke(newGameState);
    }

    public void SwitchState()
    {
        paused = !paused;
        OnGameStateChanged?.Invoke(paused);
    }
}
