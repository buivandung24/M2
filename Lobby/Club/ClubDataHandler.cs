using PlayFab;
using PlayFab.GroupsModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ClubDataHandler : MonoBehaviour
{

    // Call this method to get information about a club
    public void GetClubInfo(string clubId, Action<GroupWithRoles> onSuccess, Action<string> onError)
    {
        var request = new GetGroupRequest
        {
            Group = new EntityKey { Id = clubId, Type = "group" } // Type is always "group" for clubs
        };

        PlayFabGroupsAPI.GetGroup(request, result =>
        {
            Debug.Log("Club info retrieved successfully!");
            //onSuccess?.Invoke(result.Group);
        },
        error =>
        {
            Debug.LogError("Failed to get club info: " + error.ErrorMessage);
            onError?.Invoke(error.ErrorMessage);
        });
    }

    // Call this method to add a member to the club
    public void AddMemberToClub(string clubId, string memberId, Action onSuccess, Action<string> onError)
    {
        var request = new AddMembersRequest
        {
            Group = new EntityKey { Id = clubId, Type = "group" },
            Members = new List<EntityKey> { new EntityKey { Id = memberId, Type = "title_player_account" } } // Type is "title_player_account" for players
        };

        PlayFabGroupsAPI.AddMembers(request, result =>
        {
            Debug.Log("Member added to club successfully!");
            onSuccess?.Invoke();
        },
        error =>
        {
            Debug.LogError("Failed to add member to club: " + error.ErrorMessage);
            onError?.Invoke(error.ErrorMessage);
        });
    }

    // Call this method to remove a member from the club
    public void RemoveMemberFromClub(string clubId, string memberId, Action onSuccess, Action<string> onError)
    {
        var request = new RemoveMembersRequest
        {
            Group = new EntityKey { Id = clubId, Type = "group" },
            Members = new List<EntityKey> { new EntityKey { Id = memberId, Type = "title_player_account" } }
        };

        PlayFabGroupsAPI.RemoveMembers(request, result =>
        {
            Debug.Log("Member removed from club successfully!");
            onSuccess?.Invoke();
        },
        error =>
        {
            Debug.LogError("Failed to remove member from club: " + error.ErrorMessage);
            onError?.Invoke(error.ErrorMessage);
        });
    }

    // Use similar methods to interact with financial information and events or tournaments
    // For example, to update financial information, you may want to use PlayFab's Title Data or User Data
    // to store and retrieve the relevant data. For events or tournaments, you could use a combination of
    // group data and custom logic to manage scheduling and participation.

    // Remember to handle authentication and error checking in your actual implementation.
    // This example assumes that the current player is already authenticated with PlayFab.
}