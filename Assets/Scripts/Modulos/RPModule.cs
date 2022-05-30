namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using UnityEngine.UI;

    public class RPModule : MonoBehaviour
    {
        /// <summary> The rects recognized by the regular trainer. </summary>
        public List<Rect> rectsRegularTrainer = new List<Rect>();
        /// <summary> The rects recognized by the mirrored trainer. </summary>
        public List<Rect> rectsMirrorTrainer = new List<Rect>();
        /// <summary> Screenshots that will be used on the verification. </summary>
        public List<Texture2D> screenshots = new List<Texture2D>();
        /// <summary> Indicator for the position of the board. </summary>
        public Image indicator;
        /// <summary> Name of the regular trainer. </summary>
        public string regularTrainerName;
        /// <summary> Name of the mirrored trainer. </summary>
        public string mirrorTrainerName;
        /// <summary> Informational text. </summary>
        public Text text1;
        /// <summary> Counter for the positive verification of the regular trainer. </summary>
        public float countVerifierForRegular;
        /// <summary> Counter for the positive verification of the mirrored trainer. </summary>
        public float countVerifierForMirror;
        /// <summary> Bool that checks if the recognition has ended. </summary>
        public bool endRecognizing;
        /// <summary> Bool that checks if the screenshots were taken. </summary>
        bool thereArePhotos;
        /// <summary> Bool that checks if the rects were checked. </summary>
        public bool checkRects;
        /// <summary> The number of screenshots that will be taken and used.. </summary>
        public int qtdImages;
        /// <summary> Minimun number of positive detection for the regular trainer. </summary>
        public float minDetectedsForRegular;
        /// <summary> Minimun number of positive detection for the mirrored trainer. </summary>
        public float minDetectedsForMirror;
        // Hit percentage wanted from the trainers.
        public float percentage;
        // Hit percentage of the regular trainer.
        public float percentageOfHitsRegular;
        // Hit percentage of the regular trainer.
        public float percentageOfHitsMirror;
        // Start is called before the first frame update
        void Start()
        {
            ResetStatus();
        }

        // Update is called once per frame
        void Update()
        {
            var regularCascade = new CascadeClassifier(@"Assets/Trainer/" + 
                regularTrainerName + ".xml");
            var mirrorCascade = new CascadeClassifier(@"Assets/Trainer/" + 
                mirrorTrainerName + ".xml");

            if (Input.GetButtonDown("ActivateModule") && !thereArePhotos)
            {
                Debug.Log("STEP 1 - Taking screenshots.");
                text1.text = "Capturando imagens...";
                StartCoroutine(TakeScreenShot());
            }

            
            if (thereArePhotos)
            {
                text1.text = "Reconhecendo...";
                Debug.Log("STEP 2 - Detecting.");
                while (rectsRegularTrainer.Count < 50)
                {
                    foreach (Texture2D image in screenshots)
                    {
                        var srcImage = Unity.TextureToMat(image);
                        var grayImage = new Mat();

                        //-----------------------------------------------
                        // Convert and adjust the image.
                        Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
                        Cv2.EqualizeHist(grayImage, grayImage);

                        //-----------------------------------------------
                        // Check the image using the regular trainer.
                        var detectedOnRegular = regularCascade.DetectMultiScale(
                            image: grayImage,
                            scaleFactor: 1.1,
                            minNeighbors: 6,
                            flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage, 
                            minSize: new Size(24, 24)
                        );

                        foreach (var item in detectedOnRegular)
                        {
                            rectsRegularTrainer.Add(item);
                        }

                        //-----------------------------------------------
                        // Check the image using the mirrored trainer.
                        var detectedOnMirrored = mirrorCascade.DetectMultiScale(
                            image: grayImage,
                            scaleFactor: 1.05,
                            minNeighbors: 3,
                            flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.FindBiggestObject,
                            minSize: new Size(24, 24)
                            );

                        foreach (var item in detectedOnMirrored)
                        {
                            rectsMirrorTrainer.Add(item);
                        }
                    }

                }
                checkRects = true;
            }

            if (checkRects)
            {
                Debug.Log("STEP 3 - Checking rects.");
                thereArePhotos = false;

                foreach (Rect x in rectsRegularTrainer)
                {

                }

                //countVerifierForRegular /= 2;
                endRecognizing = true;
            }

            

            //-----------------------------------------------
            if (endRecognizing)
            {
                thereArePhotos = false;
                checkRects = false;

                minDetectedsForRegular = rectsRegularTrainer.Count * (percentage / 100);
                minDetectedsForMirror = rectsMirrorTrainer.Count * (percentage / 100);

                percentageOfHitsRegular = (rectsRegularTrainer.Count * countVerifierForRegular) / 100;

                if (percentageOfHitsRegular > minDetectedsForRegular || percentageOfHitsMirror > minDetectedsForMirror)
                {

                }
                if (rectsRegularTrainer.Count > 150)
                {
                    StartCoroutine(EndOfRecognizing(true));
                }
                else
                {
                    StartCoroutine(EndOfRecognizing(false));
                }
            }
        }

        /// <summary>
        /// Take the screenshots that will be used of the verification.
        /// </summary>
        /// <returns></returns>
        /// 

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
            indicator.gameObject.SetActive(false);
            text1.gameObject.SetActive(true);
        }

        /// <summary>
        /// Reset the values and status of the module.
        /// </summary>
        void ResetStatus()
        {
            rectsRegularTrainer.Clear();
            rectsMirrorTrainer.Clear();
            screenshots.Clear();
            text1.text = "Posicione a placa no local indicado. " +
                "Em seguida, aperte ESPAÇO para continuar.";
            countVerifierForRegular = 0;
            countVerifierForMirror = 0;
            endRecognizing = false;
            thereArePhotos = false;
            checkRects = false;
            indicator.gameObject.SetActive(true);
        }

        /// <summary>
        /// Show to the user the results of the recognition.
        /// </summary>
        /// <param name="recognized"></param>
        /// <returns></returns>
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
            yield return new WaitForSeconds(60);
            ResetStatus();
        }
        


    }
}