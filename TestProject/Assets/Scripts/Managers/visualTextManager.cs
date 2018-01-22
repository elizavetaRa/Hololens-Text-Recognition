﻿
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;

public class VisualTextManager : Singleton<VisualTextManager>
{

    public GameObject textArea;
    // public GameObject LineRenderer;
    private GazeManager gazeManager;
    private FocusManager focusManager;
    private GameObject lineRendererObject;
    private LineRenderer line1, line2, line3, line4, line5;
    // Use this for initialization
    void Start()
    {
        gazeManager = this.gameObject.GetComponentInChildren<GazeManager>();
        focusManager = this.gameObject.GetComponentInChildren<FocusManager>();
        this.lineRendererObject = this.transform.Find("LineRenderer").gameObject;
        this.line1 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line2 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line3 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line4 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line5 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        line5.startColor = Color.red;
        line5.endColor = Color.red;

        //gazeManager.FocusedObjectChanged += new GazeManager.FocusedChangedDelegate(focusChanged);
        focusManager.PointerSpecificFocusChanged += new FocusManager.PointerSpecificFocusChangedMethod(focusChanged);
        //visualizeText(dummy);
    }


    // Update is called once per frame
    void Update()
    {
        //GameObject focusedObject = gazeManager.HitObject;
        //if (focusedObject != null && focusedObject.tag != null && focusedObject.tag == "textArea" && focusedObject != currentlyFocused)
        //{
        //   // Debug.Log("found!");
        //    focusTextArea(focusedObject);
        //}

    }

    //internal void focusTextArea(GameObject focusedObject)
    //{
    //    currentlyFocused = focusedObject;
    //    focusedObject.SendMessageUpwards("OnFocus", SendMessageOptions.DontRequireReceiver);
    //}
    //internal void deFocusTextArea(GameObject deFocusedObject)
    //{
    //    //Debug.Log("call");
    //    currentlyFocused = dummy;
    //    deFocusedObject.SendMessageUpwards("OnDefocus", SendMessageOptions.DontRequireReceiver);
    //}

    static void focusChanged(IPointingSource pointer, GameObject oldObject, GameObject newObject)
    {
        if(oldObject != null)
        {
            oldObject.SendMessageUpwards("OnDefocus", SendMessageOptions.DontRequireReceiver);
        }
        if (newObject != null)
        {
            newObject.SendMessageUpwards("OnFocus", SendMessageOptions.DontRequireReceiver);
        }
        //newObject.SendMessageUpwards("OnFocus", SendMessageOptions.DontRequireReceiver);
    }




    internal void visualizeText(CameraPositionResult cameraPositionResult)
    {
        float ImageWidth = Camera.main.pixelWidth;
        float ImageHeight = Camera.main.pixelHeight;
        var ocrResult = cameraPositionResult.ocrResult; //new OcrResult("hi", new Rect(ImageWidth / 2, ImageHeight / 2, 0, 0));
        var headPosition = Camera.main.transform.position;
        float textX = 0;//ocrResult.BoundingBox.x;
        float textY = 0;//ocrResult.BoundingBox.y;
        float textWidth = ImageWidth;// ocrResult.BoundingBox.width;
        float textHeight = ImageHeight;// ocrResult.BoundingBox.height;
        var gazeDirection = Camera.main.transform.forward;
        Debug.Log(ocrResult.BoundingBox);
        Debug.Log("textWidth: " + textWidth + "; textHeight: " + textHeight + "; camHeight: " + ImageHeight + "; camWidtht: " + ImageWidth);
        Vector3[] WorldSpaceCenter = convert2DtoWorld(textX + (ImageWidth/2), textY + (ImageHeight/2), ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceTopLeft = convert2DtoWorld(textX , textY , ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceTopRight = convert2DtoWorld(textX + textWidth, textY, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceBotLeft = convert2DtoWorld(textX, textY + textHeight, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceBotRight = convert2DtoWorld(textX + textWidth, textY + textHeight, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        var Projection = cameraPositionResult.projectionMatrix;


        Vector2 ImagePosZeroToOne = new Vector2((ocrResult.BoundingBox.x + textWidth) / ImageWidth, 1.0f - ((ocrResult.BoundingBox.y+textHeight) / ImageHeight));
        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space

        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));

        //Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1)); // camera location in world space
        Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector3(0, 0, -1)); // camera location in world space
        Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space



        RaycastHit hitCenter, hitTopLeft, hitTopRight, hitBotLeft, hitBotRight, hitGaze;
        if (Physics.Raycast(WorldSpaceCenter[0], WorldSpaceCenter[1], out hitCenter) && Physics.Raycast(WorldSpaceTopLeft[0], WorldSpaceTopLeft[1], out hitTopLeft) && Physics.Raycast(WorldSpaceBotLeft[0], WorldSpaceTopRight[1], out hitTopRight) && Physics.Raycast(WorldSpaceBotLeft[0], WorldSpaceBotLeft[1], out hitBotLeft) && Physics.Raycast(WorldSpaceBotRight[0], WorldSpaceBotRight[1], out hitBotRight) && Physics.Raycast(headPosition, gazeDirection, out hitGaze))
        {
            Debug.Log("Raycasts hit!");

            //line1.SetPositions(new[] { WorldSpaceTopLeft[0], hitTopLeft.point });


            //line2.SetPositions(new[] { WorldSpaceTopRight[0], hitTopRight.point });

            //line3.SetPositions(new[] {WorldSpaceBotRight[0], hitBotRight.point });

            //line4.SetPositions(new[] { WorldSpaceBotLeft[0], hitBotLeft.point });
            line4.SetPositions(new[] { headPosition, hitGaze.point });
            line5.SetPositions(new[] { WorldSpaceCenter[0], hitCenter.point });
            

            //drawer.SetPositions(new[] { headPosition, hitTopLeft.point });
            //drawer.SetPositions(new[] { headPosition, hitTopRight.point });
            //drawer.SetPositions(new[] { headPosition, hitBotLeft.point });

            //GameObject newArea = Instantiate(textArea);
            //TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
            //float distance = (hitCenter.point - headPosition).magnitude; ;
            //Debug.Log("TopLeft: " + hitTopLeft.point + "; TopRight: " + hitTopRight.point + "; BotLeft: " + hitBotLeft.point + "; BotRight: " + hitBotRight.point + "; center: " + hitCenter.point + "; Distance: " + distance);
            ////set Text
            //visualText.text = ocrResult.Text;
            //newArea.AddComponent<MeshRenderer>();
           
            ////set Position
            //newArea.transform.position = hitCenter.point;

            ////set Rotation
            //Quaternion toQuat = Camera.main.transform.localRotation;
            //toQuat.x = 0;
            //toQuat.z = 0;
            //newArea.transform.rotation = toQuat;
            ////Debug.Log(ocrResult.BoundingBox);

            ////set Size
            //Bounds textAreaBox = newArea.GetComponent<BoxCollider>().bounds;   //.transform.Find("textAreaBox").gameObject.GetComponent<SpriteRenderer>().bounds;
            //Vector3 currentSize = textAreaBox.size;



            ////textAreaBox.Expand(2f);
            ////theSprite.OverrideGeometry();
            //float targetWidth = (hitTopLeft.point - hitTopRight.point).magnitude / (distance * 2);
            //float targetHeight =(hitTopLeft.point - hitBotLeft.point).magnitude / (distance * 2);
            //float currentWidth = currentSize.x;
            //float currentHeight = currentSize.y;
            //float scaleWidth = targetWidth / currentWidth;
            //float scaleHeight = targetHeight / currentHeight;
            //Vector3 scale = newArea.transform.localScale;
            //Debug.Log("targetwidth " + targetWidth + " ; targetHeight " + targetHeight + "\n" + "currentWidth " + currentWidth + " ; currentHeight " + currentHeight + "\n" + "scaleWidth " + scaleWidth + " ; scaleHeight " + scaleHeight + "\n" + "scale.x old: " + scale.x + " ; scale.y old: " + scale.y);
            ////Debug.Log("currentWidth " + currentWidth + " ; currentHeight " + currentHeight);
            ////Debug.Log("scaleWidth " + scaleWidth + " ; scaleHeight " + scaleHeight);
            ////Debug.Log("scale.x old: " + scale.x + " ; scale.y old: " + scale.y);
            //scale.x = targetWidth * scale.x;
            //scale.y = targetHeight * scale.y;
            //newArea.transform.localScale = scale;

            //Debug.Log("scale.x new: " + scale.x + " ; scale.y new: " + scale.y);
        }
    }

    

    public Vector3[] convert2DtoWorld(float x, float y, float imageWidth, float imageHeight, Matrix4x4 CameraToWorld, Matrix4x4 Projection )
    {
        //var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        //var Projection = cameraPositionResult.projectionMatrix;


        //Vector2 ImagePosZeroToOne = new Vector2(ocrResult.BoundingBox.x / ImageWidth, 1.0f - (ocrResult.BoundingBox.y / ImageHeight));
        //Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space

        //Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));

        ////Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1)); // camera location in world space
        //Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector3(0, 0, -1)); // camera location in world space
        //Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space

        Vector3[] result = new Vector3[2];

        Vector2 ImagePosZeroToOne = new Vector2(x / imageWidth, 1f - (y / imageHeight));
   
        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1f, 1f)); // -1 to 1 space

        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y,1));
        Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector4(0f, 0f, 0f,1f)); // camera location in world space
        Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space

        result[0] = WorldSpaceRayPoint1;
        result[1] = WorldSpaceRayPoint2;

        return result;
    }
    public static Vector3 UnProjectVector(Matrix4x4 proj, Vector3 to)
    {
        Vector3 from = new Vector3(0, 0, 0);
        var axsX = proj.GetRow(0);
        var axsY = proj.GetRow(1);
        var axsZ = proj.GetRow(2);
        from.z = to.z / axsZ.z;
        from.y = (to.y - (from.z * axsY.z)) / axsY.y;
        from.x = (to.x - (from.z * axsX.z)) / axsX.x;
        return from;
    }
}



