using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using HCoroutines;
using Promise.Framework.Utilities;
using Rubicon.Game.API.Coroutines;

namespace Rubicon.Game.API.Controllers
{
    public partial class SongCoroutineController : Node
    {
        public Dictionary<IGameCoroutine, Coroutine> SongCoroutines = new Dictionary<IGameCoroutine, Coroutine>();
        public Dictionary<IEnumerator, Coroutine> Coroutines = new Dictionary<IEnumerator, Coroutine>();

        public void Load(string songName, string stageName)
        {
            foreach (Type t in AppDomain.CurrentDomain.GetTypesWithInterface<IGameCoroutine>())
            {
                IGameCoroutine coroutine = (IGameCoroutine)t.GetConstructor(new Type[] { }).Invoke(new object[] { });
                foreach (Attribute attr in Attribute.GetCustomAttributes(t))
                {
                    if (attr is SongBind && (attr as SongBind).Song == songName || attr is StageBind && (attr as StageBind).Stage == stageName)
                        Run(coroutine);
                }
            }
        }

        public void Run(IGameCoroutine c)
        {
            SongCoroutines.Add(c, Co.Run(c.Execute()));
        }
        
        public void Run(IEnumerator c)
        {
            Coroutines.Add(c, Co.Run(c));
        }
        
        public void Kill()
        {
            foreach (Coroutine c in SongCoroutines.Values)
                c.Kill();

            foreach (Coroutine c in Coroutines.Values)
                c.Kill();
            
            SongCoroutines.Clear();
            Coroutines.Clear();
        }

        public void Kill(IGameCoroutine c)
        {
            SongCoroutines[c].Kill();
            SongCoroutines.Remove(c);
        }

        public void Kill(IEnumerator c)
        {
            Coroutines[c].Kill();
            Coroutines.Remove(c);
        }
    }
}