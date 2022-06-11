namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using UnityEngine.UI;

    public class Modulo2 : MonoBehaviour
    {
        WebCamTexture webCamTexture;
        public RawImage exibicao;
        public string nomeCam;
        public Button botao;
        public string nomeTrainerE;
        public string nomeTrainerD;
        bool pronto;
        float timer;
        float tempo;
        public Point pointAtual = new Point();
        public Point aux = new Point();
        public int verificador;
        public Rect[] detecteds;
        public GameObject cam;
        
        // Start is called before the first frame update
        void Start()
        {
            webCamTexture = new WebCamTexture();
            webCamTexture.deviceName = nomeCam;
            tempo = 5;
            exibicao.color = Color.gray;
        }

        // Update is called once per frame
        void Update()
        {
            if (this.gameObject.activeSelf)
            {
                webCamTexture.Play();
                cam.gameObject.SetActive(false);
            }
            if (webCamTexture.isPlaying)
            {
                timer += Time.deltaTime;
                if (timer > 1)
                {
                    tempo--;
                    timer = 0;
                }
                var srcImage = Unity.TextureToMat(webCamTexture);
                var grayImage = new Mat();
                Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
                Cv2.EqualizeHist(grayImage, grayImage);
                var cascadeE = new CascadeClassifier(@"Assets/Trainer/" + nomeTrainerE + ".xml");
                var cascadeD = new CascadeClassifier(@"Assets/Trainer/" + nomeTrainerD + ".xml");

                var detectedE = cascadeE.DetectMultiScale(
                    image: grayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 3,
                    flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                    minSize: new Size(30, 30)
                    );

                detecteds = detectedE;

                foreach (var item in detectedE)
                {
                    pointAtual = item.Center;
                    Scalar color = Scalar.HotPink;
                    Cv2.Rectangle(srcImage, item, color, 3);

                    if (Mathf.Approximately(item.Center.X, aux.X) && Mathf.Approximately(item.Center.Y, aux.Y))
                    {
                        verificador++;
                        break;
                    }

                    if (verificador > 120)
                    {
                        Debug.Log("Nice.");
                    }
                    aux = item.Center;
                }

                if (detectedE.Length == 0)
                {
                    verificador = 0;
                }
                //-------------------------------------------------------------
                var detectedD = cascadeD.DetectMultiScale(
                    image: grayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 3,
                    flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                    minSize: new Size(30, 30)
                    );
                
                foreach (var item in detectedD)
                {
                    pointAtual = item.Center;
                    Scalar color = Scalar.Aqua;
                    Cv2.Rectangle(srcImage, item, color, 3);

                    if (Mathf.Approximately(item.Center.X, aux.X) && Mathf.Approximately(item.Center.Y, aux.Y))
                    {
                        verificador++;
                        break;
                    }

                    if (verificador > 120)
                    {
                        Debug.Log("Nice.");
                    }
                    aux = item.Center;
                }
                 
                //var count = 1;

                if (verificador > 120)
                {
                    Debug.Log("Nice.");
                }

                if (tempo < 0)
                {
                    tempo = 5;
                }
                exibicao.texture = Unity.MatToTexture(srcImage);
            }
        }

        Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
        {
            Color32[] original = originalTexture.GetPixels32();
            Color32[] rotated = new Color32[original.Length];
            int w = originalTexture.width;
            int h = originalTexture.height;

            int iRotated, iOriginal;

            for (int j = 0; j < h; ++j)
            {
                for (int i = 0; i < w; ++i)
                {
                    iRotated = (i + 1) * h - j - 1;
                    iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                    rotated[iRotated] = original[iOriginal];
                }
            }

            Texture2D rotatedTexture = new Texture2D(h, w);
            rotatedTexture.SetPixels32(rotated);
            rotatedTexture.Apply();
            return rotatedTexture;
        }

        public Texture2D FlipTexture(Texture2D original)
        {
            int textureWidth = original.width;
            int textureHeight = original.height;

            Color[] colorArray = original.GetPixels();

            for (int j = 0; j < textureHeight; j++)
            {
                int rowStart = 0;
                int rowEnd = textureWidth - 1;

                while (rowStart < rowEnd)
                {
                    Color hold = colorArray[(j * textureWidth) + (rowStart)];
                    colorArray[(j * textureWidth) + (rowStart)] = colorArray[(j * textureWidth) + (rowEnd)];
                    colorArray[(j * textureWidth) + (rowEnd)] = hold;
                    rowStart++;
                    rowEnd--;
                }
            }

            Texture2D finalFlippedTexture = new Texture2D(original.width, original.height);
            finalFlippedTexture.SetPixels(colorArray);
            finalFlippedTexture.Apply();

            return finalFlippedTexture;
        }

        IEnumerator LimpaRect()
        {
            yield return new WaitForSeconds(5);
        }

        public void DefineCamera()
        {
            botao.gameObject.SetActive(false);
            exibicao.color = Color.white;
            this.gameObject.SetActive(true);
            this.gameObject.GetComponent<Camera>().enabled = true;
            pronto = true;
        }
    }
}