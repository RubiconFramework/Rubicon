using System;
using System.Collections;
using Godot;
using HCoroutines;
using Promise.Framework;
using Promise.Framework.Objects;
using Promise.Framework.Utilities;
using Rubicon.Game;
using Rubicon.Game.API;
using Rubicon.Game.API.Coroutines;
using Rubicon.Game.UI;

namespace HoloFunk
{
    [SongBind("astrogirl")]
    public class AstrogirlScript : IGameCoroutine
    {
        private ChartController chartCtrl = GameMaster.Instance.ChartControllers[0];
        private Vector2[] OGNotePositions = new Vector2[4];
        private float[] OGRotations = new float[4];
        
        public IEnumerator Execute()
        {
            UiBounce hudBounce = GameMaster.Instance.UI;
            hudBounce.EnableMajorBounce = false;

            chartCtrl = GameMaster.Instance.ChartControllers[0];
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                NoteLaneController noteCtrl = chartCtrl.Lanes[i];
                OGNotePositions[i] = noteCtrl.Position;
                OGRotations[i] = noteCtrl.LaneGraphic.RotationDegrees;
            }

            yield return new WaitForMeasure(8f);

            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                GameMaster.Instance.CoroutineController.Run(SineArrowX(chartCtrl.Lanes[i], ConductorUtil.MeasureToBeats(15.5f)));
                yield return new WaitTime(0.1f);
            }

            yield return new WaitForMeasure(16.25f);
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                //chartCtrl.Lanes[i].DOComplete();
                GameMaster.Instance.CoroutineController.Run(StartIntenseBeat(chartCtrl.Lanes[i], 60f, ConductorUtil.MeasureToBeats(24f)));
            }

            yield return new WaitForMeasure(24f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true));

            yield return new WaitForMeasure(26f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(false));

            yield return new WaitForMeasure(28f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true, 0.49f));

            yield return new WaitForMeasure(29f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(false, 0.49f));

            yield return new WaitForMeasure(30f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true));
            hudBounce.Bounce(0.06f);

            yield return new WaitForMeasure(30.38f);
            hudBounce.Bounce(0.06f);

            yield return new WaitForMeasure(30.75f);
            hudBounce.Bounce(0.06f);

            yield return new WaitForMeasure(32f);
            GameMaster.Instance.CoroutineController.Run(StartVerticalHUDBounce(ConductorUtil.MeasureToBeats(38.88f), 15f));

            yield return new WaitForMeasure(38.25f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(38.28f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(38.31f);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(38.44f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(38.5f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(38.56f);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(38.75f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(38.78f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(38.44f);
            SpinArrows(0.11f, false);

            yield return new WaitForMeasure(40f);
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                GameMaster.Instance.CoroutineController.Run(SineArrowX(chartCtrl.Lanes[i], ConductorUtil.MeasureToBeats(47.44f)));
                yield return new WaitTime(0.1f);
            }

            yield return new WaitForMeasure(44f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(1f, 0.03f, ConductorUtil.MeasureToBeats(46f)));

            yield return new WaitForMeasure(46f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.5f, 0.03f, ConductorUtil.MeasureToBeats(47f)));

            yield return new WaitForMeasure(47f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.25f, 0.03f, ConductorUtil.MeasureToBeats(47.44f)));

            yield return new WaitForMeasure(47.38f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(47.41f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(47.44f);
            SpinArrow(0, 0.11f, false);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(47.63f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(47.69f);
            SpinArrow(0, 0.11f, false);

            yield return new WaitForMeasure(47.81f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(47.88f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(47.94f);
            SpinArrows(0.11f, false);

            yield return new WaitForMeasure(52f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(1f, 0.03f, ConductorUtil.MeasureToBeats(54f)));

            yield return new WaitForMeasure(54f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.5f, 0.03f, ConductorUtil.MeasureToBeats(55f)));

            yield return new WaitForMeasure(55f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.25f, 0.03f, ConductorUtil.MeasureToBeats(55.38f)));

            yield return new WaitForMeasure(55.31f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(55.34f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(55.38f);
            SpinArrow(0, 0.11f, false);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(55.69f);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(55.75f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(55.88f);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(55.91f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(56f);
            SpinArrows(0.11f, false);
            hudBounce.MajorBounceBeat = 2;
            hudBounce.MinorBounceBeat = 1;
            hudBounce.EnableMajorBounce = true;
            hudBounce.EnableMinorBounce = true;
            GameMaster.Instance.CoroutineController.Run(CameraSway(ConductorUtil.MeasureToSteps(72f)));
            GameMaster.Instance.CoroutineController.Run(DelayedIntenseBeat(1f, 60f, ConductorUtil.MeasureToBeats(72f)));

            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                GameMaster.Instance.CoroutineController.Run(SineArrowY(chartCtrl.Lanes[i], ConductorUtil.MeasureToBeats(79.5f)));
                yield return new WaitTime(0.2f);
            }

            yield return new WaitForMeasure(72f);
            hudBounce.EnableMajorBounce = false;
            hudBounce.EnableMinorBounce = false;

            yield return new WaitForMeasure(76f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(1f, 0.03f, ConductorUtil.MeasureToBeats(78f)));

            yield return new WaitForMeasure(78f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.5f, 0.03f, ConductorUtil.MeasureToBeats(79f)));

            yield return new WaitForMeasure(79f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.25f, 0.03f, ConductorUtil.MeasureToBeats(79.5f)));

            yield return new WaitForMeasure(80f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true));

            yield return new WaitForMeasure(82f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(false));

            yield return new WaitForMeasure(84f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true, 0.49f));

            yield return new WaitForMeasure(85f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(false, 0.49f));

            yield return new WaitForMeasure(86f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true));

            yield return new WaitForMeasure(88f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(false));

            yield return new WaitForMeasure(90f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true));

            yield return new WaitForMeasure(92f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(false, 0.49f));

            yield return new WaitForMeasure(93f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(true, 0.49f));

            yield return new WaitForMeasure(94f);
            GameMaster.Instance.CoroutineController.Run(SineShakeNotes(false));
            hudBounce.Bounce(0.06f);

            yield return new WaitForMeasure(94.38f);
            hudBounce.Bounce(0.06f);

            yield return new WaitForMeasure(94.75f);
            hudBounce.Bounce(0.06f);

            yield return new WaitForMeasure(95.38f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(95.41f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(95.44f);
            SpinArrow(0, 0.11f, false);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(95.63f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(95.69f);
            SpinArrow(0, 0.11f, false);

            yield return new WaitForMeasure(95.81f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(95.88f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(95.94f);
            SpinArrows(0.11f, false);

            yield return new WaitForMeasure(96f);
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                GameMaster.Instance.CoroutineController.Run(SineArrowX(chartCtrl.Lanes[i], ConductorUtil.MeasureToBeats(103.38f)));
                yield return new WaitTime(0.1f);
            }

            yield return new WaitForMeasure(100f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(1f, 0.03f, ConductorUtil.MeasureToBeats(102f)));

            yield return new WaitForMeasure(102f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.5f, 0.03f, ConductorUtil.MeasureToBeats(103f)));

            yield return new WaitForMeasure(103f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.25f, 0.03f, ConductorUtil.MeasureToBeats(103.38f)));

            yield return new WaitForMeasure(108f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(1f, 0.03f, ConductorUtil.MeasureToBeats(110f)));

            yield return new WaitForMeasure(110f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.5f, 0.03f, ConductorUtil.MeasureToBeats(111f)));

            yield return new WaitForMeasure(111f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.25f, 0.03f, ConductorUtil.MeasureToBeats(111.44f)));

            yield return new WaitForMeasure(111.38f);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(111.42f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(111.46f);
            SpinArrow(0, 0.11f, false);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(111.69f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(111.75f);
            SpinArrow(0, 0.11f, false);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(111.88f);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(111.94f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(112f);
            SpinArrows(0.11f, false);
            hudBounce.EnableMajorBounce = true;
            hudBounce.EnableMinorBounce = true;
            GameMaster.Instance.CoroutineController.Run(CameraSway(ConductorUtil.MeasureToSteps(128f)));
            GameMaster.Instance.CoroutineController.Run(DelayedIntenseBeat(1f, 60f, ConductorUtil.MeasureToBeats(128f)));

            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                GameMaster.Instance.CoroutineController.Run(SineArrowY(chartCtrl.Lanes[i], ConductorUtil.MeasureToBeats(136f)));
                yield return new WaitTime(0.2f);
            }

            yield return new WaitForMeasure(128f);
            hudBounce.EnableMajorBounce = false;
            hudBounce.EnableMinorBounce = false;

            yield return new WaitForMeasure(132.25f);
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
            {
                GameMaster.Instance.CoroutineController.Run(StartIntenseBeat(chartCtrl.Lanes[i], 60f, ConductorUtil.MeasureToBeats(135.46f)));
            }
            
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(1, 0.03f, ConductorUtil.MeasureToBeats(134d)));

            yield return new WaitForMeasure(134f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.5f, 0.03f, ConductorUtil.MeasureToBeats(135f)));

            yield return new WaitForMeasure(135f);
            GameMaster.Instance.CoroutineController.Run(StartHUDBounceScale(0.25f, 0.03f, ConductorUtil.MeasureToBeats(135.46f)));

            yield return new WaitForMeasure(135.38f);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(135.42f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(135.46f);
            SpinArrow(0, 0.11f, false);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(135.69f);
            SpinArrow(1, 0.11f, false);
            SpinArrow(3, 0.11f, false);

            yield return new WaitForMeasure(135.75f);
            SpinArrow(1, 0.11f, false);

            yield return new WaitForMeasure(135.88f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(135.94f);
            SpinArrow(0, 0.11f, false);

            yield return new WaitForMeasure(135.97f);
            SpinArrow(2, 0.11f, false);

            yield return new WaitForMeasure(136f);
            SpinArrows(0.11f, false);

            /*
            yield return new WaitForMeasure(143.63f);
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
			{
				NoteLaneController curLane = chartCtrl.Lanes[i];

				DOTween.To(() => curLane.alpha, (x) => curLane.alpha = x, 0f, 1f).SetEase(Ease.OutCirc);
				curLane.transform.DOLocalMoveY((15f * (SaveData.Data.Downscroll ? 1f : -1f)), 1f).SetEase(Ease.OutCirc);

				yield return new WaitForSeconds(60f / GameMaster.Instance.BPM / 2f);
			}
			*/

            yield break;
        }

        #region Sine Arrow Stuff
        private IEnumerator SineArrowX(NoteLaneController noteCtrl, double endingBeat)
        {
            float curSineVal = 0f;
            int indexInArray = Array.IndexOf(chartCtrl.Lanes, noteCtrl);
            while (Conductor.Instance.CurrentBeat < endingBeat - 2d)
            {
                curSineVal += (float)GameMaster.Instance.GetProcessDeltaTime();

                noteCtrl.Position = new Vector2(OGNotePositions[indexInArray].X + (Mathf.Sin(Mathf.Pi * curSineVal) * 30f), noteCtrl.Position.Y);
                //noteCtrl.transform.localPosition = new Vector2(OGNotePositions[indexInArray].x + (Mathf.Sin(Mathf.PI * curSineVal) * 30f), noteCtrl.transform.localPosition.y);
                yield return null;
            }

            Tween tween = noteCtrl.CreateTween();
            tween.TweenProperty(noteCtrl, "position:x", OGNotePositions[indexInArray].X,
                (60d / Conductor.Instance.Bpm) * 2d);
            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.Play();
            
            //noteCtrl.transform.DOLocalMoveX(OGNotePositions[indexInArray].x, (60f / GameMaster.Instance.BPM) * 2f).SetEase(Ease.OutSine);
            yield break;
        }

        private IEnumerator SineArrowY(NoteLaneController noteCtrl, double endingBeat)
        {
            float curSineVal = 0f;
            int indexInArray = Array.IndexOf(chartCtrl.Lanes, noteCtrl);
            while (Conductor.Instance.CurrentBeat < endingBeat - 2f)
            {
                curSineVal += (float)GameMaster.Instance.GetProcessDeltaTime();

                noteCtrl.Position = new Vector2(noteCtrl.Position.X,
                    OGNotePositions[indexInArray].Y + (Mathf.Sin(Mathf.Pi * curSineVal) * 30f));
                //noteCtrl.transform.localPosition = new Vector2(noteCtrl.transform.localPosition.x, OGNotePositions[indexInArray].y + (Mathf.Sin(Mathf.PI * curSineVal) * 30f));
                yield return null;
            }
            
            Tween tween = noteCtrl.CreateTween();
            tween.TweenProperty(noteCtrl, "position:y", OGNotePositions[indexInArray].Y,
                (60d / Conductor.Instance.Bpm) * 2d);
            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.Play();

            //noteCtrl.transform.DOLocalMoveY(OGNotePositions[indexInArray].y, (60f / GameMaster.Instance.BPM) * 2f).SetEase(Ease.OutSine);
            yield break;
        }

        private IEnumerator SineArrowShakeY(NoteLaneController noteCtrl, double speed, float intensity, double lastingBeats)
        {
            double lastBeat = Conductor.Instance.CurrentBeat;
            float curSineVal = 0f;
            int indexInArray = Array.IndexOf(chartCtrl.Lanes, noteCtrl);

            double deltaTime = 0d;
            
            //DOTween.To(() => curSineVal, x => curSineVal = x, (2 * (Mathf.PI)) * speed, ((60 / GameMaster.Instance.BPM)) * lastingBeats * speed).SetEase(Ease.OutCubic);
            while (Conductor.Instance.CurrentBeat <= lastBeat + (lastingBeats * speed))
            {
                curSineVal = EaseOutCubic((float)deltaTime, 0f, (2 * (Mathf.Pi)) * (float)speed,
                    (float)(((60 / Conductor.Instance.Bpm)) * lastingBeats * speed));
                //double val = deltaTime / ((60 / Conductor.Instance.Bpm)) * lastingBeats * speed;
                //curSineVal = (float)(val * ((2 * (Mathf.Pi)) * speed));
                
                noteCtrl.Position = new Vector2(noteCtrl.Position.X, OGNotePositions[indexInArray].Y + (Mathf.Sin(Mathf.Pi * curSineVal) * intensity));
                //noteCtrl.transform.localPosition = new Vector2(noteCtrl.transform.localPosition.x, OGNotePositions[indexInArray].y + (Mathf.Sin(Mathf.PI * curSineVal) * intensity));
                deltaTime += GameMaster.Instance.GetProcessDeltaTime();
                yield return null;
            }

            yield break;
        }
        #endregion

        #region Intense Beat Stuff
        private IEnumerator StartIntenseBeat(NoteLaneController noteCtrl, float intensity, double endingBeat)
        {
            int lane = Array.IndexOf(chartCtrl.Lanes, noteCtrl);
            float multiplier = 1f;

            int lastGameBeat = Mathf.FloorToInt(Conductor.Instance.CurrentBeat);

            int lastBeat = -1;
            int curBeat = 0;
            while (Conductor.Instance.CurrentBeat < endingBeat)
            {
                if (lastGameBeat != Mathf.FloorToInt(Conductor.Instance.CurrentBeat))
                    curBeat++;

                // terrible i know, but too lazy :3
                if (lastBeat != curBeat && curBeat % 2 == 0)
                {
                    //noteCtrl.transform.localRotation = Quaternion.Euler(0f, 0f, 22.5f * multiplier);
                    noteCtrl.LaneGraphic.RotationDegrees = (22.5f * multiplier) + OGRotations[lane];
                    
                    switch (lane)
                    {
                        case 0:
                        {
                            noteCtrl.Position = new Vector2(OGNotePositions[lane].X - intensity, 0f);
                            break;
                        }
                        case 1:
                        {
                            noteCtrl.Position = new Vector2(OGNotePositions[lane].X - (intensity / 2f), 0f);
                            break;
                        }
                        case 2:
                        {
                            noteCtrl.Position = new Vector2(OGNotePositions[lane].X + (intensity / 2f), 0f);
                            break;
                        }
                        case 3:
                        {
                            noteCtrl.Position = new Vector2(OGNotePositions[lane].X + intensity, 0f);
                            break;
                        }
                    }

                    GameMaster.Instance.CoroutineController.Run(IntenseBeatReturn(noteCtrl));
                    multiplier *= -1f;
                }

                lastBeat = curBeat;
                lastGameBeat = Mathf.FloorToInt(Conductor.Instance.CurrentBeat);

                yield return null;
            }

            yield break;
        }

        private IEnumerator IntenseBeatReturn(NoteLaneController noteCtrl)
        {
            yield return new WaitBeats(0.5f);
            int lane = Array.IndexOf(chartCtrl.Lanes, noteCtrl);
            double time = (60f / Conductor.Instance.Bpm);

            Tween rotTween = noteCtrl.LaneGraphic.CreateTween();
            rotTween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Circ);
            rotTween.TweenProperty(noteCtrl.LaneGraphic, "rotation_degrees", OGRotations[lane], time);
            rotTween.Play();
            
            Tween posTween = noteCtrl.CreateTween();
            posTween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Circ);
            posTween.TweenProperty(noteCtrl, "position:x", OGNotePositions[lane].X, time);
            posTween.Play();
            
            //tween.TweenProperty(noteCtrl, "position:x", OGNotePositions[lane].X, time);

            //noteCtrl.transform.DOLocalRotate(Vector3.zero, time).SetEase(Ease.InOutCirc);
            //noteCtrl.transform.DOLocalMoveX(OGNotePositions[lane].x, time).SetEase(Ease.InOutCirc);

            yield break;
        }

        private IEnumerator DelayedIntenseBeat(float beatDelay, float intensity, double endingBeat)
        {
            yield return new WaitBeats(beatDelay);
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
                GameMaster.Instance.CoroutineController.Run(StartIntenseBeat(chartCtrl.Lanes[i], intensity, endingBeat));
        }
        #endregion

        #region HUD Bounce
        private IEnumerator StartVerticalHUDBounce(double endingBeat, float intensity, float speed = 1f)
        {
            UiBounce hudBounce = GameMaster.Instance.UI;
            float curSineVal = 0f;
            double deltaTime = 0d;
            int loops = (int)endingBeat - (int)Conductor.Instance.CurrentBeat - 1;
            
            //Tween hudTween = DOTween.To(() => curSineVal, x => curSineVal = x, Mathf.PI, (60 / GameMaster.Instance.BPM) * speed).SetLoops((int)endingBeat - (int)GameMaster.Instance.Beat - 1).SetEase(Ease.InOutCirc);
            while (Conductor.Instance.CurrentBeat < endingBeat - 2f && loops > 0)
            {
                hudBounce.Position = new Vector2(0f, Mathf.Abs(Mathf.Sin(curSineVal)) * intensity);
                deltaTime += GameMaster.Instance.GetProcessDeltaTime();
                curSineVal = (float)(deltaTime / ((60 / Conductor.Instance.Bpm) * speed)) * Mathf.Pi;

                if (deltaTime >= (60 / Conductor.Instance.Bpm) * speed)
                {
                    loops--;
                    deltaTime = 0;
                }
                
                yield return null;
            }

            Tween finishTween = hudBounce.CreateTween();
            finishTween.TweenProperty(hudBounce, "position", Vector2.Zero, (60d / Conductor.Instance.Bpm) * 2d);
            finishTween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Circ);
            finishTween.Play();
            //hudBounce.transform.DOLocalMove(Vector3.zero, (60f / GameMaster.Instance.BPM) * 2f).SetEase(Ease.InCirc);

            yield break;
        }

        public IEnumerator StartHUDBounceScale(int bounceBeat, float intensity, double endingBeat)
        {
            UiBounce hud = GameMaster.Instance.UI;
            while (Mathf.FloorToInt(Conductor.Instance.CurrentBeat) < endingBeat)
            {
                hud.Bounce(intensity);
                yield return new WaitBeats(bounceBeat);
            }
        }

        public IEnumerator StartHUDBounceScale(float bounceBeat, float intensity, double endingBeat)
        {
            UiBounce hud = GameMaster.Instance.UI;
            while (Conductor.Instance.CurrentBeat < endingBeat)
            {
                hud.Bounce(intensity);
                yield return new WaitBeats(bounceBeat);
            }
        }
        #endregion

        #region Stolen from Red and Black
        private void SpinArrow(int lane, double time, bool backwards)
        {
			float sign = backwards ? -1f : 1f;
			for (int i = 0; i < GameMaster.Instance.ChartControllers.Count; i++)
			{
                Control noteLane = chartCtrl.Lanes[lane].LaneGraphic;
                Tween tween = noteLane.CreateTween();
                tween.SetEase(Tween.EaseType.Out);
                tween.SetTrans(Tween.TransitionType.Circ);
                tween.TweenProperty(noteLane, "rotation_degrees", (360f * sign) + OGRotations[lane], time * 2d);
                tween.Finished += () => noteLane.RotationDegrees = OGRotations[lane];
                tween.Play();
			}
		}

        private void SpinArrows(double time, bool backwards)
        {
            for (int i = 0; i < chartCtrl.Lanes.Length; i++)
                SpinArrow(i, time, backwards);
        }

        IEnumerator CameraSway(double untilStep)
        {
			//CameraScript camera = GameObject.FindObjectOfType<CameraScript>();
            UiBounce hud = GameMaster.Instance.UI;

			while (Conductor.Instance.CurrentStep < untilStep)
            {
				//camera.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Sin(GameMaster.Instance.Beat / 1.5f));
				//hud.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Sin(GameMaster.Instance.Beat / 1.5f));
                hud.RotationDegrees = Mathf.Sin((float)Conductor.Instance.CurrentBeat / 1.5f);
                
				yield return null;
			}

			//camera.transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.InOutSine);
            Tween tween = hud.CreateTween();
            tween.TweenProperty(hud, "rotation", 0f, 0.25d);
            tween.SetEase(Tween.EaseType.InOut);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.Play();
            
			//hud.transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.InOutSine);

			yield break;
        }
        #endregion

        #region Shortcuts
        private IEnumerator SineShakeNotes(bool reverse, double time = 1f)
        {
            if (!reverse)
            {
                for (int i = 0; i < chartCtrl.Lanes.Length; i++)
                {
                    //chartCtrl.Lanes[i].DOComplete();
                    GameMaster.Instance.CoroutineController.Run(SineArrowShakeY(chartCtrl.Lanes[i], 0.75d * time, 12f, 8f));
                    yield return new WaitTime(0.05d);
                }
            }
            else
            {
                for (int i = chartCtrl.Lanes.Length - 1; i >= 0; i--)
                {
                    //chartCtrl.Lanes[i].DOComplete();
                    GameMaster.Instance.CoroutineController.Run(SineArrowShakeY(chartCtrl.Lanes[i], 0.75d * time, -12f, 8f));
                    yield return new WaitTime(0.05d);
                }
            }
        }
        #endregion

        private float EaseOutCubic(float time, float start, float change, float duration)
        {
            return start + ((1 - Mathf.Pow(1 - (time / duration), 3)) * change);
        }
    }
}