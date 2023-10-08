using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Xuan.UWP.Framework.Controls
{
    internal static class Tile3DService
    {
        private static DispatcherTimer timer = new DispatcherTimer();
        private static Random probabilisticBehaviorSelector = new Random();
        private const int waitingPipelineSteps = 3;
        private const int numberOfSimultaneousAnimations = 1;

        private const bool TrackResurrection = false;

        private static List<WeakReference> enabledPool = new List<WeakReference>();
        private static List<WeakReference> frozenPool = new List<WeakReference>();
        private static List<WeakReference> stalledPipeline = new List<WeakReference>();

        static Tile3DService()
        {
            timer.Tick += OnTimerTick;
        }

        public static void FreezeTile3D(Tile3D tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            AddReferenceToFrozenPool(wref);
            RemoveReferenceFromEnabledPool(wref);
            RemoveReferenceFromStalledPipeline(wref);
        }

        public static void UnfreezeTile3D(Tile3D tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            AddReferenceToEnabledPool(wref);
            RemoveReferenceFromFrozenPool(wref);
            RemoveReferenceFromStalledPipeline(wref);

            RestartTimer();
        }

        internal static void InitializeReference(Tile3D tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            if (tile.IsFrozen)
            {
                AddReferenceToFrozenPool(wref);
            }
            else
            {
                AddReferenceToEnabledPool(wref);
            }

            RestartTimer();
        }

        internal static void FinalizeReference(Tile3D tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            Tile3DService.RemoveReferenceFromEnabledPool(wref);
            Tile3DService.RemoveReferenceFromFrozenPool(wref);
            Tile3DService.RemoveReferenceFromStalledPipeline(wref);
        }
        private static void OnTimerTick(object sender, object e)
        {
            timer.Stop();

            for (int i = 0; i < stalledPipeline.Count; i++)
            {
                if ((stalledPipeline[i].Target as Tile3D)._stallingCounter-- == 0)
                {
                    AddReferenceToEnabledPool(stalledPipeline[i]);
                    RemoveReferenceFromStalledPipeline(stalledPipeline[i]);
                    i--;
                }
            }

            if (enabledPool.Count > 0)
            {
                for (int j = 0; j < numberOfSimultaneousAnimations; j++)
                {
                    int index = probabilisticBehaviorSelector.Next(enabledPool.Count);

                    switch ((enabledPool[index].Target as Tile3D).State)
                    {
                        case Tile3DStates.FlippedXBack:
                            if (probabilisticBehaviorSelector.Next(2) == 0)
                            {
                                (enabledPool[index].Target as Tile3D).State = Tile3DStates.FlippedX;
                            }
                            else
                            {
                                (enabledPool[index].Target as Tile3D).State = Tile3DStates.FlippedY;
                            }
                            break;
                        case Tile3DStates.FlippedX:
                            (enabledPool[index].Target as Tile3D).State = Tile3DStates.FlippedXBack;
                            break;
                        case Tile3DStates.FlippedY:
                            (enabledPool[index].Target as Tile3D).State = Tile3DStates.FlippedYBack;
                            break;
                        case Tile3DStates.FlippedYBack:
                            if (probabilisticBehaviorSelector.Next(2) == 0)
                            {
                                (enabledPool[index].Target as Tile3D).State = Tile3DStates.FlippedY;
                            }
                            else
                            {
                                (enabledPool[index].Target as Tile3D).State = Tile3DStates.FlippedX;
                            }
                            break;
                    }
                    (enabledPool[index].Target as Tile3D)._stallingCounter = waitingPipelineSteps;

                    AddReferenceToStalledPipeline(enabledPool[index]);
                    RemoveReferenceFromEnabledPool(enabledPool[index]);
                }
            }
            else if (stalledPipeline.Count == 0)
            {
                return;
            }

            timer.Interval = TimeSpan.FromMilliseconds(probabilisticBehaviorSelector.Next(10, 31) * 100);
            timer.Start();
        }


        private static void RestartTimer()
        {
            if (!timer.IsEnabled)
            {
                timer.Interval = TimeSpan.FromMilliseconds(2500);
                timer.Start();
            }
        }

        private static void AddReferenceToStalledPipeline(WeakReference tile)
        {
            if (!ContainsTarget(stalledPipeline, tile.Target))
            {
                stalledPipeline.Add(tile);
            }
        }

        private static void RemoveReferenceFromEnabledPool(WeakReference tile)
        {
            RemoveTarget(enabledPool, tile.Target);
        }
        private static void AddReferenceToEnabledPool(WeakReference tile)
        {
            if (!ContainsTarget(enabledPool, tile.Target))
            {
                enabledPool.Add(tile);
            }
        }

        private static void RemoveReferenceFromStalledPipeline(WeakReference tile)
        {
            RemoveTarget(stalledPipeline, tile.Target);
        }
        private static void AddReferenceToFrozenPool(WeakReference tile)
        {
            if (!ContainsTarget(frozenPool, tile.Target))
            {
                frozenPool.Add(tile);
            }
        }
        private static void RemoveReferenceFromFrozenPool(WeakReference tile)
        {
            RemoveTarget(frozenPool, tile.Target);
        }
        private static bool ContainsTarget(List<WeakReference> list, Object target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Target == target)
                {
                    return true;
                }
            }
            return false;
        }
        private static void RemoveTarget(List<WeakReference> list, Object target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Target == target)
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

    }
    internal enum Tile3DStates
    {
        FlippedY,
        FlippedYBack,
        FlippedX,
        FlippedXBack
    }
}
