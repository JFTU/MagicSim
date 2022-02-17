using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace MagicSim_Feature_DrawingAndTesting
{
    public class Draw : MonoBehaviour
    {
        [SerializeField]
        private GameObject LinePrefab;

        private GameObject currentLine = null;

        private LineRenderer lineRenderer;

        [SerializeField]
        private List<Vector3> Line = new List<Vector3>();

        [SerializeField]
        private Texture2D Texture;

        public Texture2D texture { get => Texture; private set => Texture = value; }

        [SerializeField]
        private NNModel model;
        public NNModel Model { get => model; }


        private Model runModel;
        public Model RunModel { get => runModel; private set => runModel = value; }

        Camera cam;

        IWorker worker;
        // Start is called before the first frame update
        void Start()
        {
            RunModel = ModelLoader.Load(model);
            worker = WorkerFactory.CreateWorker(RunModel);
        }

        void CreateLine(Vector3 startingPosition)
        {
            currentLine = Instantiate(LinePrefab, transform);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            Line.Clear();
            Line.Add(startingPosition);
            Line.Add(startingPosition);
            print(Line[0]);
            lineRenderer.SetPosition(0, Line[0]);
            lineRenderer.SetPosition(1, Line[1]);


        }

        void UpdateLine(Vector3 newPosition)
        {
            Line.Add(newPosition);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPosition);
        }

        void DestroyLine()
        {
            Destroy(currentLine);
            currentLine = null;
        }

        public void Test()
        {
            var temp = ScreenCapture.CaptureScreenshotAsTexture();
            temp = ScaleTexture(temp, 28, 28);
            var temp2 = new Texture2D(28, 28, TextureFormat.R16, false);
            temp2.SetPixels(temp.GetPixels());
            temp2.Apply();
            //File.WriteAllBytes(Application.dataPath + "/Testing.png",temp2.EncodeToPNG());

            Tensor input = new Tensor(temp2);
            worker.Execute(input);
            var output = worker.PeekOutput();
            var tempOut = output.ToReadOnlyArray();
            foreach (float f in tempOut)
            {
                print(f);
            }

            print(output.ArgMax().GetValue(0));

            input.Dispose();

            DestroyLine();
        }

        //This code is not mine. I got it from http://jon-martin.com/?p=114
        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] rpixels = result.GetPixels(0);
            float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
            float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other == HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out var pose))
            {
                CreateLine(pose.Position);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other == HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out var pose))
            {
                if (currentLine != null)
                {

                    if (Vector2.Distance(pose.Position, Line[Line.Count - 1]) > 0.000000001f)
                    {
                        UpdateLine(pose.Position);
                    }
                }
            }
        }
    }
}
