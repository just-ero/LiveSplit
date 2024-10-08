﻿using System;
using System.Collections.Generic;
using System.Linq;

using LiveSplit.Model;

namespace LiveSplit.Web.SRL;

public class SRLRaceInfo : IRaceInfo
{
    private readonly dynamic _data;

    public SRLRaceInfo(dynamic data)
    {
        _data = data;
        foreach (dynamic entrant in _data.entrants.Properties.Values)
        {
            if (entrant.time >= 0)
            {
                Finishes++;
            }

            if (entrant.statetext == "Forfeit")
            {
                Forfeits++;
            }
        }
    }

    public string Id => _data.id;
    public string GameName => _data.game.name;
    public int Finishes { get; set; } = 0;
    public int Forfeits { get; set; } = 0;
    public int NumEntrants => _data.numentrants;
    public string Goal => _data.goal;
    public int State => _data.state;
    public int Starttime => _data.time;
    public string GameId => _data.game.abbrev;

    public bool IsParticipant(string username)
    {
        IEnumerable<string> racers = ((IEnumerable<string>)_data.entrants.Properties.Keys).Select(x => x.ToLower());
        return racers.Contains((username ?? "").ToLower());
    }

    public IEnumerable<string> LiveStreams
    {
        get
        {
            foreach (dynamic entrant in _data.entrants.Properties.Values)
            {
                if (entrant.statetext == "Forfeit" || entrant.time >= 0)
                {
                    continue;
                }

                yield return entrant.twitch;
            }
        }
    }
}
