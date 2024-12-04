using UnityEngine;

public class CharacterModel
{
    private readonly CapsuleCollider _collider;
    private readonly CharacterConfig _config;
    private bool _isInitialized;
    private GameObject _girlModel;
    private GameObject _boyModel;
    private Transform _parentTransform;

    public CharacterModel(CapsuleCollider collider, CharacterConfig config, Transform parentTransform)
    {
        _collider = collider ?? throw new System.ArgumentNullException(nameof(collider));
        _config = config ?? throw new System.ArgumentNullException(nameof(config));
        _parentTransform = parentTransform;

        // Validate configuration
        _config.Validate();

        // Au lieu d'instancier, on cherche les modèles existants
        FindExistingModels();
        InitializeModel();
    }

    private void FindExistingModels()
    {
        // Chercher dans les enfants du parent
        Transform modelParent = _parentTransform.Find("ModelParent");
        if (modelParent != null)
        {
            _girlModel = modelParent.Find("GirlModel")?.gameObject;
            _boyModel = modelParent.Find("BoyModel")?.gameObject;
        }

        // Validation
        if (_girlModel == null || _boyModel == null)
        {
            Debug.LogError("Models not found in ModelParent! Make sure you have GirlModel and BoyModel as children.");
        }
    }

    private void InitializeModel()
    {
        if (_isInitialized)
            return;

        if (_config.useGirlModel)
        {
            SetupGirlModel();
        }
        else
        {
            SetupBoyModel();
        }

        _isInitialized = true;
    }

    private void AdjustModelPosition(GameObject model, Vector3 size)
    {
        if (model != null)
        {
            // Ajuste la position pour que les pieds soient au niveau du sol
            // et que le centre du modèle corresponde au centre du collider
            float heightOffset = size.y / 2f;
            model.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void SetupGirlModel()
    {
        if (_girlModel != null && _boyModel != null)
        {
            _girlModel.SetActive(true);
            _boyModel.SetActive(false);
            UpdateColliderSize(_config.girlSize);
        }
        else
        {
            Debug.LogError("Missing model references in CharacterConfig!");
        }
    }

    private void SetupBoyModel()
    {
        if (_girlModel != null && _boyModel != null)
        {
            _boyModel.SetActive(true);
            _girlModel.SetActive(false);
            UpdateColliderSize(_config.boySize);
        }
        else
        {
            Debug.LogError("Missing model references in CharacterConfig!");
        }
    }

    private void UpdateColliderSize(Vector3 size)
    {
        if (_collider != null)
        {
            _collider.radius = size.x / 2f;
            _collider.height = size.y;

            // Update the center to half the height
            _collider.center = new Vector3(0f, size.y / 2f, 0f);
        }
    }

    // Optional: Method to reset the model state if needed
    public void Reset()
    {
        InitializeModel();
    }

    public void Cleanup()
    {
        if (_girlModel!= null)
            GameObject.Destroy(_girlModel);
        if (_boyModel != null)
            GameObject.Destroy(_boyModel);
    }
}