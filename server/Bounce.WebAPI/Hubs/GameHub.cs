using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Bounce.WebAPI.Hubs
{
    public class Entity
    {
        public float x  {get;set;}
        public float y {get;set;}

        public Guid guid {get;set;}
    }

    public class GameState
    {
        public ConcurrentDictionary<Guid, Entity> Entities {get;set;} = new ConcurrentDictionary<Guid, Entity>();

        public void AddEntity(Guid guid, Entity entity)
        {
            if(!Entities.ContainsKey(guid))
                Entities.TryAdd(guid, entity);       
        }

        public bool TryGetEntity(Guid guid, out Entity entity) => Entities.TryGetValue(guid, out entity);

        public void RemoveEntity(Guid guid)
        {
            if(Entities.ContainsKey(guid))
                Entities.TryRemove(guid, out var entity);       
        }
    }

    public class Game
    {
        public GameState GameState {get;set;} = new GameState();

        public ICollection<Entity> Entities => GameState.Entities.Values;

        private static Timer _timer = new Timer(33);

        public string SerializeGameState => JsonConvert.SerializeObject(GameState);
        
        public void OnUserConnected(Guid guid)
        {
            GameState.AddEntity(guid, new Entity{guid = guid });
        }

        public void OnUserPosition(Guid guid, float x, float y)
        {
            if(GameState.TryGetEntity(guid, out var entity))
            {
                entity.x = x;
                entity.y = y;
            }
        }

        public void OnUserDisconnected(Guid guid)
        {
            GameState.RemoveEntity(guid);
        }

        //public GameHub()
        //{
        //    Init();
        //    Start();
        //}

        //public void Start()
        //{
        //    _timer.Start();
        //}

        //public void Stop()
        //{
        //    _timer.Stop();
        //}

        //public void Init()
        //{
        //    _timer.Elapsed += async (sender, args) =>
        //    {
        //        foreach(var key in _game.GameState.Entities.Keys)
        //        {
        //            var entity = _game.GameState.Entities[key];
        //            await Clients.All.ClientUpdateUserPosition(key, entity.x, entity.y);
        //        } 
        //    }; 
        //}
    }

    public class GameHub : Hub
    {
        private static Game _game = new Game();

        public async Task ServerUpdateUserConnected(Guid guid)
        {
            _game.OnUserConnected(guid);
            foreach(var entity in _game.Entities)
            {
                await Clients.All.ClientUpdateUserConnected(entity.guid);
            }
        }

        public async Task ServerUpdateUserPosition(Guid guid, float x, float y)
        {
            //_game.OnUserPosition(guid, x, y);
            //return Task.FromResult(0);
            await Clients.All.ClientUpdateUserPosition(guid, x, y);
        }

        public async Task ServerUpdateUserDisconnected(Guid guid)
        {
            _game.OnUserDisconnected(guid);
            await Clients.All.ClientUpdateUserDisconnected(guid);
        }
    }
}