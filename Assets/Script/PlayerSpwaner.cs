using Fusion;
using UnityEngine;

public class PlayerSpwaner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(_playerPrefab, new Vector3(0, 5, 0), Quaternion.identity, player);
        }
    }
}

