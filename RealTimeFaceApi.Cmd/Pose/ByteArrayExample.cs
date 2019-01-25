using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TensorFlow;

public class ByteArrayExample {
    PoseNet posenet = new PoseNet();
    PoseNet.Pose[] poses;

    //public GameObject glgo;
    //private GLRenderer gl;

    // Use this for initialization
    void Start () {
		
		var heatmap = GetResourcesToFloats("heatmaps",33,33,17);
		var offsets = GetResourcesToFloats("offsets",33,33,34);
		var displacementsFwd = GetResourcesToFloats("displacementsFwd",33,33,32);
		var displacementsBwd = GetResourcesToFloats("displacementsBwd",33,33,32);

        poses = posenet.DecodeMultiplePoses(
            heatmap, offsets,
            displacementsFwd,
            displacementsBwd,
            outputStride: 16, maxPoseDetections: 15,
            scoreThreshold: 0.5f, nmsRadius: 20);

        //gl = glgo.GetComponent<GLRenderer>();

    }

	public static float[,,,] GetResourcesToFloats(string name, int y,int x,int z) {
        // TODO
		//TextAsset binary = Resources.Load (name) as TextAsset;
        byte[] bytes = new byte[0];
		var floatValues = GetByteToFloat(bytes);
		var tensor = GetFloatToTensor(floatValues, y, x, z);
		return (float[,,,])tensor.GetValue();
	}

	public static TFTensor GetFloatToTensor(float[] floatValues, int y,int x,int z) {
		TFShape shape = new TFShape(1, y, x, z);
		var tensor = TFTensor.FromBuffer(shape, floatValues, 0, floatValues.Length);
		return tensor;
	}

	public static float[] GetByteToFloat(byte[] byteArray) {
		var floatArray = new float[byteArray.Length / 4];
		Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
		return floatArray;
	}
	
	//// Update is called once per frame
	//void Update () {

 //   }
    //public void OnRenderObject()
    //{
    //    gl.DrawResults(poses);
    //}
}
