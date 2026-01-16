using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace MyVRConstructor
{
    public class AutonomousConstructionMenu : MonoBehaviour
    {
        [Header("=== НОВАЯ СТРУКТУРА (ПОДКЛЮЧИТЕ В ИНСПЕКТОРЕ) ===")]
        [SerializeField] private GameObject managerObject; // Объект "Manager" с компонентом Actions
        [SerializeField] private GameObject createSystem; // Объект со скриптом Create.cs
        [SerializeField] private GameObject combineSystem; // Объект со скриптом Combine.cs
        [SerializeField] private GameObject resizeSystem; // Объект со скриптом Resize.cs
        
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
        
        [Header("=== MODE SELECTION ===")]
        [SerializeField] private Button createModeButton;
        [SerializeField] private Button combineModeButton;
        [SerializeField] private Button resizeModeButton;
        [SerializeField] private Button decombineModeButton;
        [SerializeField] private Button resize_Button;
        [SerializeField] private Button clearButton;

        [Header("=== SIZE SETTINGS ===")]
        [SerializeField] private Slider sizeSlider;
        [SerializeField] private Text sizeText;
        [SerializeField] private float minSize = 0.1f;
        [SerializeField] private float maxSize = 2.0f;
        
        [Header("=== CONTROLLER REFERENCES ===")]
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;
        [SerializeField] private float spawnDistance = 0.5f;

        public InputActionProperty leftIinput;
        public InputActionProperty rightIinput;

        // Текущие настройки
        private int selectedPrimitiveIndex = 0;
        private int selectedColorIndex = 0;
        private float currentSize = 1.0f;
        private Color currentColor = Color.white;
        
        // Ссылки на компоненты новой системы
        private Actions actionsManager;
        private Create createScript;
        private Combine combineScript;
        private Resize resizeScript;
        
        private Transform xrCamera;
        private bool isMenuVisible = false;
        
        void Start()
        {
            InitializeReferences();
            SetupUI();
            HideMenu();
        }
        
        void Update()
        {
            if (leftIinput.action.IsPressed() && rightIinput.action.IsPressed())
            {
                ShowMenu();
            }
            if (isMenuVisible)
                UpdateMenuPosition();
        }
        
        #region Инициализация
        
        private void InitializeReferences()
        {
            xrCamera = Camera.main?.transform;
            
            // Находим Manager и компонент Actions
            if (managerObject == null)
                managerObject = GameObject.Find("Manager");
            
            if (managerObject != null)
            {
                actionsManager = managerObject.GetComponent<Actions>();
                if (actionsManager == null)
                    Debug.LogWarning("Actions component not found on Manager");
            }
            
            // Находим скрипт Create
            if (createSystem == null)
                createSystem = GameObject.Find("CreateSystem") ?? managerObject;
            
            if (createSystem != null)
            {
                createScript = createSystem.GetComponent<Create>();
                if (createScript == null)
                    Debug.LogWarning("Create script not found");
            }
            
            // Находим скрипт Combine
            if (combineSystem == null)
                combineSystem = GameObject.Find("CombineSystem") ?? managerObject;
            
            // Находим скрипт Resize
            if (resizeSystem == null)
                resizeSystem = GameObject.Find("ResizeSystem") ?? managerObject;
            
            if (resizeSystem != null)
            {
                resizeScript = resizeSystem.GetComponent<Resize>();
                if (resizeScript == null)
                    Debug.LogWarning("Resize script not found");
            }
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
            
            // Настройка кнопок режимов
            if (createModeButton != null)
                createModeButton.onClick.AddListener(() => SetMode("create"));
            
            if (combineModeButton != null)
                combineModeButton.onClick.AddListener(() => SetMode("combine"));
            
            if (resizeModeButton != null)
                resizeModeButton.onClick.AddListener(() => SetMode("resize"));

            if (resize_Button != null)
                resize_Button.onClick.AddListener(()=>SetMode("resize_"));
            
            if (decombineModeButton != null)
                decombineModeButton.onClick.AddListener(() => SetMode("decombine"));

            if (clearButton != null)
                clearButton.onClick.AddListener(() => SetMode("clear"));
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
                
                // Устанавливаем соответствующий флаг в Create.cs
                if (createScript != null)
                {
                    // Сбрасываем все флаги
                    createScript.cube = false;
                    createScript.sphere = false;
                    createScript.cylinder = false;
                    createScript.cone = false;
                    createScript.torus = false;
                    createScript.prism = false;
                    
                    // Устанавливаем выбранный примитив
                    string prefabName = primitivePrefabs[index].name.ToLower();
                    if (prefabName.Contains("cube")) createScript.cube = true;
                    else if (prefabName.Contains("sphere")) createScript.sphere = true;
                    else if (prefabName.Contains("cylinder")) createScript.cylinder = true;
                    else if (prefabName.Contains("cone")) createScript.cone = true;
                    else if (prefabName.Contains("torus")) createScript.torus = true;
                    else if (prefabName.Contains("prism")) createScript.prism = true;
                }
                
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
                
                // Устанавливаем цвет в Create.cs
                if (createScript != null)
                {
                    createScript.color = new Vector3(currentColor.r, currentColor.g, currentColor.b);
                }
                
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
        
        #region Управление режимами
        
        private void SetMode(string mode)
        {
            if (actionsManager == null) return;
            
            // Сброс всех режимов
            actionsManager.create = false;
            actionsManager.resize = 0;
            actionsManager.combine = 0;
            
            // Установка выбранного режима
            switch (mode.ToLower())
            {
                case "create":
                    actionsManager.create = true;
                    HighlightModeButton(createModeButton);
                    Debug.Log("Mode: CREATE - Выберите примитив и нажмите на контроллер");
                    break;
                    
                case "combine":
                    // Для combine используется специальная логика через скрипт Combine
                    // Просто устанавливаем начальное состояние
                    actionsManager.combine = 1;
                    HighlightModeButton(combineModeButton);
                    Debug.Log("Mode: COMBINE - Возьмите объект и отпустите рядом с другим");
                    break;
                    
                case "resize":
                    // resize = 1 для увеличения, можно добавить переключатель
                    actionsManager.resize = 1;
                    HighlightModeButton(resizeModeButton);
                    Debug.Log("Mode: RESIZE - Наведите на сторону объекта и нажмите триггер");
                    break;

                case "resize_":
                    actionsManager.resize = -1;
                    HighlightModeButton(resize_Button);
                    break;

                case "decombine":
                    // Для разъединения (это может быть отдельная логика)
                    // Пока что просто сбрасываем combine
                    actionsManager.combine = -1;
                    HighlightModeButton(decombineModeButton);
                    Debug.Log("Mode: DECOMBINE - Удаление соединений");
                    // Можно добавить специальную логику для разъединения
                    break;

                case "clear":
                    actionsManager.create = false;
                    actionsManager.resize = 0;
                    actionsManager.combine = 0;
                    break;
            }
            
            // Сброс подсветки других кнопок
            ResetModeButtonsHighlight(mode);
        }
        
        private void HighlightModeButton(Button button)
        {
            if (button == null) return;
            
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Зеленый для активного режима
            }
        }
        
        private void ResetModeButtonsHighlight(string currentMode)
        {
            Button[] modeButtons = { createModeButton, combineModeButton, resizeModeButton, decombineModeButton };
            string[] modeNames = { "create", "combine", "resize", "decombine" };
            
            for (int i = 0; i < modeButtons.Length; i++)
            {
                if (modeButtons[i] != null && modeNames[i] != currentMode)
                {
                    Image buttonImage = modeButtons[i].GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = Color.white;
                    }
                }
            }
        }
        
        #endregion
        
        #region Создание объектов
        
        public void CreateObjectAtController(bool isLeftController)
        {
            if (createScript == null || actionsManager == null) return;
            
            Transform controller = isLeftController ? leftController : rightController;
            if (controller == null) return;
            
            // Активируем режим создания
            actionsManager.create = true;
            
            // Создание произойдет автоматически в Create.Update()
            // когда будет нажат триггер
            Debug.Log($"Ready to create at {controller.name}. Press trigger to create.");
        }
        
        public GameObject SpawnPrimitiveAtPosition(Vector3 position, Quaternion rotation)
        {
            if (primitivePrefabs.Count == 0 || selectedPrimitiveIndex >= primitivePrefabs.Count) 
                return null;
            
            // Создаем объект
            GameObject primitive = Instantiate(
                primitivePrefabs[selectedPrimitiveIndex], 
                position, 
                rotation
            );
            
            // Применяем настройки
            ApplySettingsToObject(primitive);
            return primitive;
        }
        
        private void ApplySettingsToObject(GameObject obj)
        {
            if (obj == null) return;
            
            // Применяем цвет
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = currentColor;
            }
            
            // Применяем размер
            obj.transform.localScale = Vector3.one * currentSize;
            
            // Добавляем необходимые компоненты
            EnsureRequiredComponents(obj);
        }
        
        private void EnsureRequiredComponents(GameObject obj)
        {
            // Rigidbody для физики
            if (!obj.TryGetComponent<Rigidbody>(out var rb))
                rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            
            // Тег для совместимости с Resize.cs и другими системами
            obj.tag = "PhysObj";
            
            // Добавляем компонент Combine для возможности соединения
            if (!obj.TryGetComponent<Combine>(out _))
            {
                obj.AddComponent<Combine>();
            }
            
            // Добавляем коллайдеры если их нет
            if (obj.GetComponent<Collider>() == null)
            {
                // В зависимости от типа объекта добавляем соответствующий коллайдер
                if (obj.GetComponent<MeshFilter>() != null)
                {
                    obj.AddComponent<MeshCollider>();
                }
                else
                {
                    obj.AddComponent<BoxCollider>();
                }
            }
        }
        
        #endregion
        
        #region Публичный API
        
        public void ShowMenu() 
        { 
            isMenuVisible = true;
            menuCanvas.gameObject.SetActive(true);
        }
        
        public void HideMenu() 
        { 
            isMenuVisible = false;
            if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(false);
        }
        
        public bool IsMenuVisible => isMenuVisible;
        
        // Быстрые команды для кнопок
        public void QuickCreateCube() => SelectPrimitive(0);
        public void QuickCreateSphere() => SelectPrimitive(1);
        public void QuickCreateCylinder() => SelectPrimitive(2);
        
        #endregion
    }
}