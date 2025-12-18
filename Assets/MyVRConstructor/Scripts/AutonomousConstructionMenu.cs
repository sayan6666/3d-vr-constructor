using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace MyVRConstructor
{
    public class AutonomousConstructionMenu : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] Canvas menuCanvas;
        [SerializeField] RectTransform menuPanel;
        [SerializeField] float menuDistance = 1.5f;
        [SerializeField] float menuScale = 0.5f;
        
        [Header("Menu Tabs")]
        [SerializeField] GameObject shapesTab;
        [SerializeField] GameObject colorsTab;
        [SerializeField] GameObject toolsTab;
        
        [Header("Shape Buttons")]
        [SerializeField] List<GameObject> shapePrefabs;
        [SerializeField] List<Button> shapeButtons;
        
        [Header("Color Buttons")]
        [SerializeField] List<Color> availableColors;
        [SerializeField] List<Button> colorButtons;
        
        [Header("Size Settings")]
        [SerializeField] Slider sizeSlider;
        [SerializeField] Text sizeText;
        [SerializeField] float minSize = 0.1f;
        [SerializeField] float maxSize = 2.0f;
        
        [Header("Input Actions")]
        [SerializeField] InputActionProperty toggleMenuAction;
        [SerializeField] InputActionProperty createObjectLeftAction;
        [SerializeField] InputActionProperty createObjectRightAction;
        [SerializeField] InputActionProperty navigateMenuAction;
        
        [Header("Spawn Positions")]
        [SerializeField] Transform leftController;
        [SerializeField] Transform rightController;
        [SerializeField] float spawnDistance = 0.5f;
        
        [Header("Visual Feedback")]
        [SerializeField] ParticleSystem creationParticles;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip menuOpenSound;
        [SerializeField] AudioClip buttonClickSound;
        [SerializeField] AudioClip createSound;
        
        // Текущие настройки
        private GameObject currentShapePrefab;
        private Color currentColor = Color.white;
        private float currentSize = 1.0f;
        private bool isMenuVisible = false;
        private Transform xrCamera;
        private int selectedShapeIndex = 0;
        private int selectedColorIndex = 0;
        
        void Start()
        {
            xrCamera = Camera.main?.transform;
            
            // Настройка канваса
            if (menuCanvas != null)
            {
                menuCanvas.worldCamera = Camera.main;
                menuCanvas.planeDistance = 0.1f;
                SetMenuVisibility(false);
            }
            
            // Настройка префабов по умолчанию
            if (shapePrefabs.Count > 0)
                currentShapePrefab = shapePrefabs[0];
            
            // Настройка кнопок фигур
            for (int i = 0; i < shapeButtons.Count && i < shapePrefabs.Count; i++)
            {
                int index = i;
                shapeButtons[i].onClick.AddListener(() => SelectShape(index));
                
                // Устанавливаем иконку или текст
                Text buttonText = shapeButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = shapePrefabs[i].name;
            }
            
            // Настройка кнопок цветов
            for (int i = 0; i < colorButtons.Count && i < availableColors.Count; i++)
            {
                int index = i;
                colorButtons[i].onClick.AddListener(() => SelectColor(index));
                
                // Устанавливаем цвет на кнопке
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
            }
            
            UpdateSizeDisplay();
            
            // Включаем Input Actions
            EnableInputActions();
            
            // Автоматический поиск контроллеров
            FindControllersAutomatically();
        }
        
        void Update()
        {
            // Обработка ввода
            HandleInput();
            
            // Позиционирование меню
            if (isMenuVisible)
                PositionMenu();
        }
        
        void HandleInput()
        {
            // Переключение меню
            if (toggleMenuAction.action.WasPressedThisFrame())
            {
                ToggleMenu();
            }
            
            // Создание объекта левым контроллером
            if (createObjectLeftAction.action.WasPressedThisFrame())
            {
                CreateObjectAtController(true);
            }
            
            // Создание объекта правым контроллером
            if (createObjectRightAction.action.WasPressedThisFrame())
            {
                CreateObjectAtController(false);
            }
            
            // Навигация по меню (если видимо)
            if (isMenuVisible)
            {
                Vector2 navigation = navigateMenuAction.action.ReadValue<Vector2>();
                if (navigation != Vector2.zero)
                {
                    NavigateMenu(navigation);
                }
            }
        }
        
        void ToggleMenu()
        {
            isMenuVisible = !isMenuVisible;
            SetMenuVisibility(isMenuVisible);
            
            if (isMenuVisible)
            {
                PlaySound(menuOpenSound);
                Debug.Log("Menu opened");
            }
            else
            {
                Debug.Log("Menu closed");
            }
        }
        
        void SetMenuVisibility(bool visible)
        {
            if (menuCanvas != null)
                menuCanvas.gameObject.SetActive(visible);
            
            // Простая анимация
            if (visible && menuPanel != null)
            {
                menuPanel.localScale = Vector3.zero;
                StartCoroutine(AnimateMenuAppearance());
            }
        }
        
        System.Collections.IEnumerator AnimateMenuAppearance()
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;
            
            while (elapsed < duration)
            {
                menuPanel.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            menuPanel.localScale = endScale;
        }
        
        void PositionMenu()
        {
            if (xrCamera == null || menuCanvas == null) return;
            
            // Позиция перед камерой
            Vector3 targetPosition = xrCamera.position + xrCamera.forward * menuDistance;
            
            // Плавное следование
            menuCanvas.transform.position = Vector3.Lerp(
                menuCanvas.transform.position,
                targetPosition,
                Time.deltaTime * 8f
            );
            
            // Поворот к камере
            menuCanvas.transform.LookAt(
                2 * menuCanvas.transform.position - xrCamera.position
            );
            
            // Масштабирование относительно расстояния
            float distance = Vector3.Distance(menuCanvas.transform.position, xrCamera.position);
            float scale = Mathf.Clamp(menuScale / distance, 0.3f, 1.5f);
            menuCanvas.transform.localScale = Vector3.one * scale;
        }
        
        void SelectShape(int index)
        {
            if (index >= 0 && index < shapePrefabs.Count)
            {
                selectedShapeIndex = index;
                currentShapePrefab = shapePrefabs[index];
                
                // Визуальная обратная связь
                HighlightButton(shapeButtons, index);
                PlaySound(buttonClickSound);
                
                Debug.Log($"Selected shape: {currentShapePrefab.name}");
            }
        }
        
        void SelectColor(int index)
        {
            if (index >= 0 && index < availableColors.Count)
            {
                selectedColorIndex = index;
                currentColor = availableColors[index];
                
                // Визуальная обратная связь
                HighlightButton(colorButtons, index);
                PlaySound(buttonClickSound);
                
                Debug.Log($"Selected color: {currentColor}");
            }
        }
        
        void SetSize(float size)
        {
            currentSize = Mathf.Clamp(size, minSize, maxSize);
            UpdateSizeDisplay();
        }
        
        void UpdateSizeDisplay()
        {
            if (sizeText != null)
                sizeText.text = $"Size: {currentSize:F2}x";
        }
        
        void HighlightButton(List<Button> buttons, int selectedIndex)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Image buttonImage = buttons[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = (i == selectedIndex) 
                        ? new Color(0.3f, 0.6f, 1f, 1f)  // Выделенный цвет
                        : Color.white;                    // Обычный цвет
                }
            }
        }
        
        void NavigateMenu(Vector2 direction)
        {
            // Простая навигация стрелками/джойстиком
            // Можно реализовать выбор элементов без клика
            Debug.Log($"Menu navigation: {direction}");
        }
        
        void CreateObjectAtController(bool isLeftController)
        {
            if (currentShapePrefab == null)
            {
                Debug.LogWarning("No shape selected!");
                return;
            }
            
            Transform controller = isLeftController ? leftController : rightController;
            if (controller == null)
            {
                Debug.LogWarning("Controller not found!");
                return;
            }
            
            // Позиция создания
            Vector3 spawnPosition = controller.position + controller.forward * spawnDistance;
            Quaternion spawnRotation = controller.rotation;
            
            // Создание объекта
            GameObject newObj = Instantiate(currentShapePrefab, spawnPosition, spawnRotation);
            
            // Применение настроек
            ApplySettingsToObject(newObj);
            
            // Визуальная и звуковая обратная связь
            PlayCreationEffects(spawnPosition);
            
            Debug.Log($"Created {currentShapePrefab.name} at {controller.name}");
        }
        
        void ApplySettingsToObject(GameObject obj)
        {
            if (obj == null) return;
            
            // Применяем цвет
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Создаем новый материал, чтобы не менять префаб
                Material newMaterial = new Material(renderer.material);
                newMaterial.color = currentColor;
                renderer.material = newMaterial;
            }
            
            // Применяем размер
            obj.transform.localScale = Vector3.one * currentSize;
            
            // Добавляем физику если нет
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
                rb = obj.AddComponent<Rigidbody>();
            
            rb.useGravity = true;
            
            // Добавляем тег для совместимости
            obj.tag = "PhysObj";
        }
        
        void PlayCreationEffects(Vector3 position)
        {
            // Частицы
            if (creationParticles != null)
            {
                creationParticles.transform.position = position;
                creationParticles.Play();
            }
            
            // Звук
            PlaySound(createSound);
        }
        
        void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        void FindControllersAutomatically()
        {
            // Автоматический поиск контроллеров если не назначены
            if (leftController == null)
            {
                GameObject left = GameObject.Find("left");
                if (left != null) leftController = left.transform;
            }
            
            if (rightController == null)
            {
                GameObject right = GameObject.Find("right");
                if (right != null) rightController = right.transform;
            }
            
            // Или ищем по тегу
            if (leftController == null || rightController == null)
            {
                GameObject[] controllers = GameObject.FindGameObjectsWithTag("Controller");
                foreach (var controller in controllers)
                {
                    if (controller.name.Contains("left", System.StringComparison.OrdinalIgnoreCase))
                        leftController = controller.transform;
                    else if (controller.name.Contains("right", System.StringComparison.OrdinalIgnoreCase))
                        rightController = controller.transform;
                }
            }
        }
        
        void EnableInputActions()
        {
            toggleMenuAction.action.Enable();
            createObjectLeftAction.action.Enable();
            createObjectRightAction.action.Enable();
            navigateMenuAction.action.Enable();
        }
        
        void OnDisable()
        {
            toggleMenuAction.action.Disable();
            createObjectLeftAction.action.Disable();
            createObjectRightAction.action.Disable();
            navigateMenuAction.action.Disable();
        }
        
        // Публичные методы для внешнего доступа
        public void ShowMenu() => SetMenuVisibility(true);
        public void HideMenu() => SetMenuVisibility(false);
        public bool IsMenuVisible => isMenuVisible;
        
        public GameObject CreateObjectAtPosition(Vector3 position, Quaternion rotation)
        {
            if (currentShapePrefab == null) return null;
            
            GameObject newObj = Instantiate(currentShapePrefab, position, rotation);
            ApplySettingsToObject(newObj);
            PlayCreationEffects(position);
            
            return newObj;
        }
    }
}