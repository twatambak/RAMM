namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using UnityEngine.UI;

    public class Modulo3 : MonoBehaviour
    {
        public List<Rect> rectsRegularTrainer = new List<Rect>();
        public List<Rect> rectsMirrorTrainer = new List<Rect>();
        public List<Texture2D> screenshots = new List<Texture2D>();
        public Image indicator;
        public string regularTrainerName;
        public string mirrorTrainerName;
        public Text text1;
        public int countVerifierForRegular;
        public int countVerifierForMirror;
        public bool endRecognizing;
        bool thereArePhotos;
        int actualPos;
        public bool checkRects;
        int rectCountForRegular;
        int rectCountForMirror;
        public int qtdImages;
        public int percentual;
        // Start is called before the first frame update
        void Start()
        {
            rectsRegularTrainer.Clear();
            rectsMirrorTrainer.Clear();
            screenshots.Clear();
            text1.text = "Posicione a placa no local indicado. Em seguida, aperte ESPAÇO para continuar.";
            countVerifierForRegular = 0;
            countVerifierForMirror = 0;
            endRecognizing = false;
            thereArePhotos = false;
            actualPos = 0;
            checkRects = false;
            rectCountForRegular = 1;
            rectCountForMirror = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("ActivateModule") && !thereArePhotos)
            {
                text1.text = "Capturando imagens...";
                StartCoroutine(TakeScreenShot());
            }
            //-----------------------------------------------
            if (thereArePhotos)
            {
                text1.text = "Reconhecendo...";

                var regularCascade = new CascadeClassifier(@"Assets/Trainer/" + regularTrainerName + ".xml");
                var mirrorCascade = new CascadeClassifier(@"Assets/Trainer/" + mirrorTrainerName + ".xml");

                while (actualPos < screenshots.Count)
                {
                    //-----------------------------------------------
                    // Aplica a textura da screenshot atual na variável base para uso.
                    var srcImage = Unity.TextureToMat(screenshots[actualPos]);
                    var grayImage = new Mat();

                    //-----------------------------------------------
                    // Converte e ajusta a imagem.
                    Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
                    Cv2.EqualizeHist(grayImage, grayImage);

                    //-----------------------------------------------
                    // Regular trainer (trainer com a placa normal)
                    var detectedOnRegular = regularCascade.DetectMultiScale(
                        image: grayImage,
                        scaleFactor: 1.05,
                        minNeighbors: 6,
                        flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                        minSize: new Size(24, 24)
                        );

                    foreach (var item in detectedOnRegular)
                    {
                        rectsRegularTrainer.Add(item);
                    }

                    //-----------------------------------------------
                    // Mirror trainer (imagem mirrorada)
                    var detectedOnMirrored = mirrorCascade.DetectMultiScale(
                        image: grayImage,
                        scaleFactor: 1.05,
                        minNeighbors: 3,
                        flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                        minSize: new Size(30, 30)
                        );

                    foreach (var item in detectedOnMirrored)
                    {
                        rectsMirrorTrainer.Add(item);
                    }

                    //-----------------------------------------------
                    actualPos++;
                }
                checkRects = true;
            }

            if (checkRects)
            {
                if (rectsRegularTrainer.Count > 0)
                {
                    //-----------------------------------------------
                    // Verifica os rects reconhecidos pelo trainer regular.
                    while (rectCountForRegular < (rectsRegularTrainer.Count - 2))
                    {
                        //if (Mathf.Approximately(rectsRegularTrainer[rectCountForRegular].Center.X, rectsRegularTrainer[rectCountForRegular + 1].Center.X) && Mathf.Approximately(rectsRegularTrainer[rectCountForRegular].Center.Y, rectsRegularTrainer[rectCountForRegular + 1].Center.Y) || (Mathf.Approximately(rectsMirrorTrainer[rectCountForMirror].Center.X, rectsMirrorTrainer[rectCountForMirror - 1].Center.X) && Mathf.Approximately(rectsMirrorTrainer[rectCountForMirror].Center.Y, rectsMirrorTrainer[rectCountForMirror - 1].Center.Y)))
                        for (int j = 1; j < rectsRegularTrainer.Count - 1; j++)
                        {
                            if (j == rectCountForRegular)
                            {
                                j++;
                            }
                            if (rectsRegularTrainer[rectCountForRegular].Contains(rectsRegularTrainer[j]))
                            {
                                countVerifierForRegular++;
                            }
                        }
                        rectCountForRegular++;
                    }
                }

                if (rectsMirrorTrainer.Count > 0)
                {
                    //-----------------------------------------------
                    // Verifica os rects reconhecidos pelo trainer espelhado.
                    while (rectCountForMirror < (rectsMirrorTrainer.Count - 1))
                    {
                        if (Mathf.Approximately(rectsMirrorTrainer[rectCountForMirror].Center.X, rectsMirrorTrainer[rectCountForMirror + 1].Center.X) 
                            && 
                            Mathf.Approximately(rectsMirrorTrainer[rectCountForMirror].Center.Y, rectsMirrorTrainer[rectCountForMirror + 1].Center.Y))
                        {
                            countVerifierForMirror++;
                        }
                        rectCountForMirror++;
                    }
                }


                endRecognizing = true;
            }

            if (endRecognizing)
            {
                //-----------------------------------------------
                // Realiza a contagem do número de valores.
                if (countVerifierForRegular >= percentual || countVerifierForMirror >= percentual)
                {
                    StartCoroutine(EndOfRecognizing(true));
                }
                else
                {
                    StartCoroutine(EndOfRecognizing(false));
                }
            }
        }

        IEnumerator TakeScreenShot()
        {
            indicator.gameObject.SetActive(false);
            yield return new WaitForSeconds(2);
            text1.text = "Aguarde...";
            for (int i = 0; i < qtdImages; i++)
            {
                yield return new WaitForEndOfFrame();
                Texture2D aux = ScreenCapture.CaptureScreenshotAsTexture();
                screenshots.Add(aux);
            }
            thereArePhotos = true;
            checkRects = true;
            endRecognizing = false;
            indicator.gameObject.SetActive(false);
            text1.gameObject.SetActive(true);
        }

        IEnumerator EndOfRecognizing(bool recognized)
        {
            text1.gameObject.SetActive(true);
            if (recognized)
            {
                text1.text = "Placa reconhecida. Pressione ENTER para continuar.";

            }
            else
            {
                text1.text = "A placa não foi reconhecida. Por favor, tente novamente.";
            }
            yield return new WaitForSeconds(10);
            endRecognizing = false;
            rectsRegularTrainer.Clear();
            rectsMirrorTrainer.Clear();
            screenshots.Clear();
            countVerifierForRegular = 0;
            countVerifierForMirror = 0;
            thereArePhotos = false;
            actualPos = 0;
            checkRects = false;
            rectCountForRegular = 1;
            rectCountForMirror = 1;
            text1.text = "Posicione a placa no local indicado. Em seguida, aperte ESPAÇO para continuar.";
            indicator.gameObject.SetActive(true);
        }
    }
}