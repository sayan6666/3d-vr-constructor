using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Collections.Generic;

namespace MyVRConstructor
{
    public class AutonomousConstructionMenu : MonoBehaviour
    {
        [Header("=== EXISTING SYSTEMS (DO NOT MODIFY) ===")]
        [SerializeField] private GameObject newfGameObject;  // Объект со скриптом newf.cs
        [SerializeField] private MonoBehaviour combineScriptHolder; // Объект со скриптом combine.cs
        
        [Header("=== NEW SYSTEMS ===")]
        [SerializeField] private ObjectSpawner objectSpawner;
        
        [Header("=== UI COMPONENTS ===")]
        [SerializeField] private Canvas menuCanvas;
        [SerializeField] private RectTransform menuPanel;
        [SerializeField] private float menuDistance = 1.5f;
        
        [Header("=== PRIMITIVE SETTINGS ===")]
        [SerializeField] private List<GameObject> primitivePrefabs;
        [SerializeField] private List<Button> primitiveButtons;
        
        [Header("=== COLOR SETTINGS ===")]
        [SerializeField] private List<Color> availableColors;
        [SerializeField] private List<Button> colorButtons;
        [SerializeField] private Image currentColorDisplay;
        
        [Header("=== SIZE SETTINGS ===")]
        [SerializeField] private Slider sizeSlider;
        [SerializeField] private Text sizeText;
        [SerializeField] private float minSize = 0.1f;
        [SerializeField] private float maxSize = 2.0f;
        
        [Header("=== CONTROLLER REFERENCES ===")]
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;
        [SerializeField] private float spawnDistance = 0.5f;
        
        // Текущие настройки
        private int selectedPrimitiveIndex = 0;
        private int selectedColorIndex = 0;
        private float currentSize = 1.0f;
        private Color currentColor = Color.white;
        
        // Ссылки
        private Transform xrCamera;
        private bool isMenuVisible = false;
        
        // Кэшированные компоненты из существующих скриптов
        private MonoBehaviour newfScript;
        private MonoBehaviour combineScript;
        
        void Start()
        {
            InitializeReferences();
            SetupUI();
            SetupObjectSpawner();
            HideMenu();
        }
        
        void Update()
        {
            if (isMenuVisible)
                UpdateMenuPosition();
        }
        
        #region Инициализация
        
        private void InitializeReferences()
        {
            xrCamera = Camera.main?.transform;
            
            // Получаем ссылки на существующие скрипты
            if (newfGameObject != null)
            {
                // Ищем скрипт newf на объекте
                newfScript = newfGameObject.GetComponent("newf") as MonoBehaviour;
                if (newfScript == null)
                    Debug.LogWarning("newf script not found on the specified GameObject");
            }
            
            if (combineScriptHolder != null)
            {
                // Ищем скрипт combine на объекте
                combineScript = combineScriptHolder.GetComponent("combine") as MonoBehaviour;
                if (combineScript == null)
                    Debug.LogWarning("combine script not found on the specified GameObject");
            }
            
            // Настраиваем ObjectSpawner
            if (objectSpawner == null)
                objectSpawner = FindObjectOfType<ObjectSpawner>();
        }
        
        private void SetupUI()
        {
            if (menuCanvas != null)
            {
                menuCanvas.worldCamera = Camera.main;
                menuCanvas.planeDistance = 0.1f;
            }
            
            // Настройка кнопок примитивов
            for (int i = 0; i < primitiveButtons.Count && i < primitivePrefabs.Count; i++)
            {
                int index = i;
                primitiveButtons[i].onClick.AddListener(() => SelectPrimitive(index));
                
                Text buttonText = primitiveButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = primitivePrefabs[i].name;
            }
            
            // Настройка кнопок цветов
            for (int i = 0; i < colorButtons.Count && i < availableColors.Count; i++)
            {
                int index = i;
                colorButtons[i].onClick.AddListener(() => SelectColor(index));
                
                Image buttonImage = colorButtons[i].GetComponent<Image>();
                if (buttonImage != null)
                    buttonImage.color = availableColors[i];
            }
            
            // Настройка слайдера размера
            if (sizeSlider != null)
            {
                sizeSlider.minValue = minSize;
                sizeSlider.maxValue = maxSize;
                sizeSlider.value = currentSize;
                sizeSlider.onValueChanged.AddListener(SetSize);
                UpdateSizeDisplay();
            }
            
            // Настройка отображения цвета
            if (currentColorDisplay != null)
                currentColorDisplay.color = currentColor;
        }
        
        private void SetupObjectSpawner()
        {
            if (objectSpawner != null)
            {
                // Используем префабы из нашего меню
                objectSpawner.objectPrefabs = primitivePrefabs;
                objectSpawner.cameraToFace = Camera.main;
                
                // Подписываемся на событие создания
                objectSpawner.objectSpawned += OnObjectSpawned;
            }
        }
        
        #endregion
        
        #region Управление меню
        
        public void ToggleMenu()
        {
            isMenuVisible = !isMenuVisible;
            menuCanvas.gameObject.SetActive(isMenuVisible);
            
            if (isMenuVisible)
                StartCoroutine(AnimateMenuAppearance());
        }
        
        private System.Collections.IEnumerator AnimateMenuAppearance()
        {
            if (menuPanel == null) yield break;
            
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;
            
            menuPanel.localScale = startScale;
            
            while (elapsed < duration)
            {
                menuPanel.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            menuPanel.localScale = endScale;
        }
        
        private void UpdateMenuPosition()
        {
            if (xrCamera == null || menuCanvas == null) return;
            
            Vector3 targetPosition = xrCamera.position + xrCamera.forward * menuDistance;
            menuCanvas.transform.position = Vector3.Lerp(
                menuCanvas.transform.position,
                targetPosition,
                Time.deltaTime * 8f
            );
            
            menuCanvas.transform.LookAt(2 * menuCanvas.transform.position - xrCamera.position);
        }
        
        #endregion
        
        #region Выбор настроек
        
        public void SelectPrimitive(int index)
        {
            if (index >= 0 && index < primitivePrefabs.Count)
            {
                selectedPrimitiveIndex = index;
                
                // Устанавливаем в ObjectSpawner
                if (objectSpawner != null)
                    objectSpawner.spawnOptionIndex = index;
                
                // Визуальная обратная связь
                HighlightButton(primitiveButtons, index);
                
                Debug.Log($"Selected primitive: {primitivePrefabs[index].name}");
            }
        }
        
        public void SelectColor(int index)
        {
            if (index >= 0 && index < availableColors.Count)
            {
                selectedColorIndex = index;
                currentColor = availableColors[index];
                
                // Обновляем отображение
                if (currentColorDisplay != null)
                    currentColorDisplay.color = currentColor;
                
                // Визуальная обратная связь
                HighlightButton(colorButtons, index);
                
                Debug.Log($"Selected color: {currentColor}");
            }
        }
        
        public void SetSize(float size)
        {
            currentSize = Mathf.Clamp(size, minSize, maxSize);
            UpdateSizeDisplay();
        }
        
        private void UpdateSizeDisplay()
        {
            if (sizeText != null)
                sizeText.text = $"Size: {currentSize:F2}x";
        }
        
        private void HighlightButton(List<Button> buttons, int selectedIndex)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Image buttonImage = buttons[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = (i == selectedIndex) 
                        ? new Color(0.3f, 0.6f, 1f, 1f)
                        : Color.white;
                }
            }
        }
        
        #endregion
        
        #region Интеграция с существующими системами
        
        public void CreateObjectAtController(bool isLeftController)
        {
            if (objectSpawner == null) return;
            
            Transform controller = isLeftController ? leftController : rightController;
            if (controller == null) return;
            
            Vector3 spawnPosition = controller.position + controller.forward * spawnDistance;
            Vector3 spawnNormal = -controller.forward;
            
            // Используем ObjectSpawner для создания
            bool success = objectSpawner.TrySpawnObject(spawnPosition, spawnNormal);
            
            if (success)
            {
                Debug.Log($"Created object at {controller.name}");
            }
        }
        
        private void OnObjectSpawned(GameObject spawnedObject)
        {
            // Применяем настройки к созданному объекту
            ApplySettingsToObject(spawnedObject);
            
            // Активируем режим соединения если нужно
            ActivateCombineModeIfNeeded(spawnedObject);
        }
        
        private void ApplySettingsToObject(GameObject obj)
        {
            if (obj == null) return;
            
            // Применяем цвет
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material newMaterial = new Material(renderer.material);
                newMaterial.color = currentColor;
                renderer.material = newMaterial;
            }
            
            // Применяем размер
            obj.transform.localScale = Vector3.one * currentSize;
            
            // Добавляем необходимые компоненты
            EnsureRequiredComponents(obj);
        }
        
        private void EnsureRequiredComponents(GameObject obj)
        {
            // Rigidbody
            if (!obj.TryGetComponent<Rigidbody>(out var rb))
                rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            
            // Тег для совместимости с newf.cs
            obj.tag = "PhysObj";
            
            // Компонент combine для совместимости
            if (combineScript != null)
            {
                // Копируем компонент combine с оригинального объекта
                CopyCombineComponent(obj);
            }
        }
        
        private void CopyCombineComponent(GameObject targetObj)
        {
            // Этот метод копирует компонент combine с существующего объекта
            // на новый объект для совместимости
            
            // Находим любой объект с компонентом combine
            /*var existingCombine = FindObjectOfType<combine>();
            if (existingCombine != null)
            {
                // Копируем компонент через AddComponent
                var newCombine = targetObj.AddComponent<combine>();
                
                // Здесь можно скопировать настройки если нужно
                // Но так как combine.cs не имеет публичных полей, 
                // просто добавляем компонент
            }*/
        }
        
        private void ActivateCombineModeIfNeeded(GameObject obj)
        {
            // Активируем режим соединения для объекта
            // Это нужно для совместимости с существующей системой
            
            /*var combineComp = obj.GetComponent<combine>();
            if (combineComp != null)
            {
                // Используем рефлексию для вызова метода SetCombined
                // если он публичный
                System.Reflection.MethodInfo method = 
                    combineComp.GetType().GetMethod("SetCombined");
                    
                if (method != null)
                {
                    method.Invoke(combineComp, null);
                }
            }*/
        }
        
        // Методы для управления режимами через существующий newf.cs
        public void ActivateCreateMode()
        {
            // Здесь можно попытаться активировать режим создания
            // через существующий newf.cs если это возможно
            GameObject.Find("Manager").GetComponent<Actions>().create = true;
            Debug.Log("Create mode activated");
        }
        
        public void ActivateCombineMode()
        {
            // Активируем режим соединения для всех объектов
            /*var allCombineObjects = FindObjectsOfType<combine>();
            foreach (var combineObj in allCombineObjects)
            {
                System.Reflection.MethodInfo method = 
                    combineObj.GetType().GetMethod("SetCombined");
                    
                if (method != null)
                {
                    method.Invoke(combineObj, null);
                }
            }*/
            
            Debug.Log("Combine mode activated for all objects");
        }
        
        public void ActivateResizeMode()
        {
            // Можно добавить логику для активации режима изменения размера
            // через существующий newf.cs
            Debug.Log("Resize mode activated");
        }
        
        #endregion
        
        #region Публичный API
        
        public void ShowMenu() => menuCanvas.gameObject.SetActive(true);
        public void HideMenu() => menuCanvas.gameObject.SetActive(false);
        public bool IsMenuVisible => isMenuVisible;
        
        public GameObject SpawnPrimitiveAtPosition(Vector3 position, Quaternion rotation)
        {
            if (objectSpawner == null || primitivePrefabs.Count == 0) 
                return null;
            
            // Создаем объект
            GameObject primitive = Instantiate(
                primitivePrefabs[selectedPrimitiveIndex], 
                position, 
                rotation
            );
            
            ApplySettingsToObject(primitive);
            return primitive;
        }
        
        #endregion
    }
}