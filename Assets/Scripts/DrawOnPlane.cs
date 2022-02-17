using UnityEngine;
using Unity.Barracuda;

//This Script is depreciated and only stored for documentation purposes, please see Draw for the new drawing script.
public class DrawOnPlane : MonoBehaviour
{
    void Start()
    {
        copyMade = false;
        runModel = ModelLoader.Load(model);
    }
    bool copyMade;

    public NNModel model;
    public Model runModel;

    [SerializeField]
    private Texture2D Texture;

    public Texture2D texture { get => Texture; private set => Texture = value; }
    // Update is called once per frame
    private void Update()
    {
        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
    void createCopy()
    {
        texture = Instantiate(texture);
        copyMade = true;
    }

    private void Draw(Vector2 mousePos)
    {
        GetComponent<MeshRenderer>().material.SetTextureOffset(0, new Vector2(32,32));
        if (!copyMade)
        {
            createCopy();
        }
        print(texture.texelSize);

        mousePos.x = texture.width/2 + mousePos.x * -texture.width;
        mousePos.y = texture.height/2 + mousePos.y * -texture.height;
        texture.SetPixel((int)mousePos.x, (int)mousePos.y, Color.white);
    }

    public void Test()
    {
        var tensor = new TextureAsTensorData(texture);
        TensorShape shape = new TensorShape(-1, 28, 28, 0);
        Tensor input = new Tensor(shape, tensor);
        var worker = WorkerFactory.CreateWorker(runModel);

        worker.Execute(input);

        var output = worker.PeekOutput();

        print(output.ArgMax().GetValue(0));

        input.Dispose();
        createCopy();
    }

    private void OnTriggerStay(Collider other)
    {
        print(other.transform.localPosition);
        Draw(new Vector2(other.transform.position.x, other.transform.position.y));
    }

    private void OnTriggerExit(Collider other)
    {
        texture.Apply();
    }
}
