namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using UnityEngine.UI;

    public class Modulo : MonoBehaviour
    {
        WebCamTexture webcamTexture;
        public string nomeCam;
        public RawImage exibicao;
        public string nomeTrainerE;
        public string nomeTrainerD;
        public Text texto1;
        bool pronto;
        float timer;
        public float tempo;
        public Point pointAtual = new Point();
        public Point aux = new Point();
        public int cont;
        public int verificador;
        public bool verificado;
        public GameObject cam;

        // Start is called before the first frame update
        void Start()
        {
            webcamTexture = new WebCamTexture();
            webcamTexture.deviceName = nomeCam;
            tempo = 10;
            timer = 0;
            exibicao.color = Color.white;
            if (cam.gameObject.activeSelf)
            {
                cam.gameObject.SetActive(false);
                webcamTexture.Play();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (webcamTexture.isPlaying)
            {
                timer += Time.deltaTime;
                if (timer > 1)
                {
                    tempo--;
                    timer = 0;
                }

                var cascadeE = new CascadeClassifier(@"Assets/Trainer/" + nomeTrainerE + ".xml");
                var cascadeD = new CascadeClassifier(@"Assets/Trainer/" + nomeTrainerD + ".xml");
                
                var srcImage = Unity.TextureToMat(webcamTexture);
                var grayImage = new Mat();

                Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
                Cv2.EqualizeHist(grayImage, grayImage);

                var detectedE = cascadeE.DetectMultiScale(
                    image: grayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 3,
                    flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                    minSize: new Size(30, 30)
                    );

                foreach (var item in detectedE)
                {
                    pointAtual = item.Center;
                    Scalar color = Scalar.HotPink;
                    Cv2.Rectangle(srcImage, item, color, 3);
                    aux = item.Center;
                }

                //-----------------------------------------------

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
                    Scalar color = Scalar.Aquamarine;
                    Cv2.Rectangle(srcImage, item, color, 3);
                    if (Mathf.Approximately(item.Center.X, aux.X) && Mathf.Approximately(item.Center.Y, aux.Y))
                    {
                        cont++;
                        verificador++;
                        texto1.gameObject.SetActive(true);
                        texto1.text = "Reconhecendo...";
                        break;
                    }
                    aux = item.Center;
                }

                if (verificador > 120)
                {
                    verificado = true;
                }
                exibicao.texture = Unity.MatToTexture(srcImage);
                //-----------------------------------------------

                if (tempo < 0 && !verificado)
                {
                    texto1.gameObject.SetActive(true);
                    texto1.text = "A placa não foi reconhecida. Há algo errado.";
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        Debug.LogError("Erro! TEMPO 0 NÃO VERIFICADO");
                    }
                } 
                else if (tempo < 0 && verificado)
                {
                    texto1.gameObject.SetActive(true);
                    texto1.text = "A placa foi reconhecida! Aperte ENTER para continuar.";
                    Debug.LogError("Erro! TEMPO 0 VERIFICADO");
                    /*if (Input.GetKeyDown(KeyCode.Space))
                    {
                        this.gameObject.SetActive(false);
                        cam.gameObject.SetActive(true);
                        Debug.LogError("ALERTA! ALERTA!");
                    }*/
                }


                if (detectedE.Length == 0)
                {
                    verificador = 0;
                    texto1.gameObject.SetActive(false);
                    texto1.text = "";
                }
            }

        }

    }
}