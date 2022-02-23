using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject[] Cards;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject DropZone;

    List<GameObject> deck = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        DropZone = GameObject.Find("DropZone");
    }
    
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        int[] card_amounts = new int[] {8,3,3,3,3,4,12,12,12,4,4,4,3,2,2,5,3,5,2,3,2,3,3,3,2};
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < card_amounts[i]; j++)
            {   
                deck.Add(Cards[i]);
            }
        }

        Debug.Log("Deck Size: " + deck.Count);
    }
    
    [Command]
    public void CmdDealCards()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject card = Instantiate(deck[Random.Range(0, deck.Count-1)], new Vector2(0,0), Quaternion.identity);
            Debug.Log("Deck Size: " + deck.Count);
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");
        }
    }

    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");

        if (isServer)
        {
            UpdateTurnsPlayed();
        }
    }

    [Server]
    void UpdateTurnsPlayed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateTurnsPlayed();
        RpcLogToClients("Turns Played: " + gm.TurnsPlayed);

    }
    //Rpc = Remote Procedure call
    [ClientRpc]
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "Dealt")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
                card.GetComponent<CardFlip>().Flip();
            }
        }
        else if (type == "Played")
        {
            card.transform.SetParent(DropZone.transform, false);
            if (!hasAuthority)
            {
                card.GetComponent<CardFlip>().Flip();
            }
        }
    }

    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }

    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity opponentIdentity = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(opponentIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetSelfCard()
    {
        Debug.Log("Targeted by Self!");
    }

    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
        Debug.Log("Targeted by Other!");
    }

    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);
    }

    [ClientRpc]
    void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().NumberOfClicks++;
        Debug.Log("This card has been clicked" + card.GetComponent<IncrementClick>().NumberOfClicks + " times.");
    }
}
