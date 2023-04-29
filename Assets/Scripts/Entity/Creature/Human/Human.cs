using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Omivorous
{
#nullable enable
    public GameObject? spouse = null;

#nullable disable
    public string firstName { get; set; }
    private string _firstNameMeaning { get; set; }
    public string lastName { get; set; }
    private string _lastNameMeaning { get; set; }

    private const int MAX_WEIGHT = 100; // Потом можно будет более гибко настроить

#nullable enable
    private House? _home = null;

#nullable disable
    [SerializeField] public GameObject startHome;
    [SerializeField] public RuntimeAnimatorController[] genderAnimatorController;
    [SerializeField] public List<GameObject> huntableAnimals;

    private HomeNecessity _homeNecessity = new HomeNecessity();

    ////////////////////////////////////
    //Конструктор и взаимодействие с пользователем
    ///////////////////////////////////

    public override void Start()
    {
        base.Start();
        name = "человек:";
        (_firstNameMeaning, firstName) = Namer.MakeName(3);
        (_lastNameMeaning, lastName) = Namer.MakeName(3);
        _animator.runtimeAnimatorController = gender ? genderAnimatorController[0] : genderAnimatorController[1];
    }

    protected override void OnMouseDown()
    {
        string message = "";
        message += $"Это {name}\n";
        message += $"{firstName} {lastName}\n";
        message += $"{GetStateStr()}\n";
        message += $"Пол: {GetGenderStr()}\n";
        message += $"Позиция: {transform.position}\n";
        message += $"Сытость: {_satiety.CurrentStatePercent()}%\n";
        message += $"Порог сытости: {_satiety.ThresholdPercent()}%\n";
        message += $"Секс: {_sexNecessity.CurrentStatePercent()}%\n";
        message += $"Порог для поиска партнера: {_sexNecessity.ThresholdPercent()}%\n";
        message += $"Дом: {(_homeNecessity.CurrentStatePercent() == 100 ? "Есть" : "Нет")}\n";
        message += $"{GetMarriageStr()}\n";
        SendText(message);
    }

    protected override string GetGenderStr()
    {
        return gender ? "Мужской" : "Женский";
    }

    private string GetMarriageStr()
    {
        if (gender == FEMALE)
        {
            if (spouse == null)
            {
                return "Не замужем";
            }

            else return $"Замужем за {spouse.GetComponent<Human>().firstName} {spouse.GetComponent<Human>().lastName}";
        }

        else
        {
            if (spouse == null)
            {
                return "Не женат";
            }

            else return $"Женат на {spouse.GetComponent<Human>().firstName} {spouse.GetComponent<Human>().lastName}";

        }

    }

    private string GetStateStr()
    {
        switch (_currentState)
        {
            case STATE.SEEK_FOR_FOOD:
                return "Ищет еду";

            case STATE.SEEK_FOR_PARTNER:
                if (spouse != null) return "Собирается исполнять супружеский долг";
                return "Ищет любви";

            case STATE.SEEK_FOR_HOMEPLACE:
                return "Ищет место для жилья";

            case STATE.CHILL:
                return "Бродит без дела";

            default:
                return "НЕИЗВЕСТНОЕ СОСТОЯНИЕ";
        }
    }

    //////////////////Проверки и вычисления параметров////////////////////////
    private bool CanMarry(GameObject humanToMarry)
    {
        var humanScript = humanToMarry.GetComponent<Human>();

        if (humanScript._currentState == STATE.SEEK_FOR_PARTNER
        && spouse == null
        && humanScript.gender != this.gender
        && humanScript.spouse == null) return true;

        return false;
    }
    private int CalculateWeight()
    {
        int result = 0;

        foreach (var item in Inventory)
        {
            var itemScript = item.GetComponent<IThing>();
            result += itemScript.weight;
        }

        return result;
    }
    protected override void CheckNecessities()
    /*Проверка потребностей. Если есть неудовлетворенные потребности, состояние животного изменится, и оно начнет их удовлетворять*/
    {
        if (!_satiety.isSatisfied())
        {
            if (!_satiety.IsCritical())
            {
                _currentState = STATE.SEEK_FOR_FOOD;
                return;
            }
            else
            {
                Die();
            }
        }

        if (!_homeNecessity.isSatisfied())
        {
            _currentState = STATE.SEEK_FOR_HOMEPLACE;
            return;
        }

        if (!_sexNecessity.isSatisfied())
        {
            if (!_satiety.IsCritical())
            {
                _currentState = STATE.SEEK_FOR_PARTNER;
                return;
            }
            else
            {
                /*NB ЕЩЕ не допилил, что делать, если долго не сексил*/
                Debug.Log("Forever alone...");
            }
        }

        _currentState = STATE.CHILL;
    }

    //////////////////Действия////////////////////////

    private void Kill(Creature creatureToKill)
    {
        creatureToKill.Die();
    }
    public void SetHome(House newHome)
    {
        if (_home != null)
        {
            _home.RemoveInhabitor(this);
        }
        _home = newHome;
        _home.AddInhabitor(this);
    }
    private void BuildHome()
    {
        SetHome(Instantiate(startHome, transform.position, Quaternion.identity).GetComponent<House>());
    }
    private void EatFoodFromInventory(GameObject[] foodList)
    {
        int i = 0;
        while (!_satiety.isSatisfied() && i < foodList.Length)
        {
            var currentFoodInc = foodList[i].GetComponent<IEdible>().satietyIncrement;
            _satiety.Increase(currentFoodInc);
            Inventory.Remove(foodList[i]);
            i++;
        }
    }
    private void Marry(GameObject humanToMarry)
    {
        if (gender == MALE)
        {
            this.spouse = humanToMarry;

            var spouseScript = this.spouse.GetComponent<Human>();
            spouseScript.spouse = this.gameObject;
            spouseScript.SetHome(this._home);
        }
    }
    private void CollectFruitsFromTree(GameObject tree)
    {
        var treeScript = tree.GetComponent<Tree>();

        List<GameObject> collected = new List<GameObject>();

        foreach (var fruit in treeScript.Inventory) //NB Осторожно! когда дерево будет содержать бревна, нужно будет подкорректировать
        {
            var fruitScript = fruit.GetComponent<IThing>();
            if (CalculateWeight() + fruitScript.weight < MAX_WEIGHT) collected.Add(fruit);
            else break;
        }

        foreach (var fruit in collected)
        {
            treeScript.Inventory.Remove(fruit);
        }

        Inventory.AddRange(collected);
    }

    private void CollectMeatFromCorpse(Corpse corpse)
    {
        List<GameObject> collected = new List<GameObject>();

        foreach(var meat in corpse.Inventory)//NB Осторожно! когда у трупа будут кости, нужно будет подкорректировать
        {
            var meatScript = meat.GetComponent<IThing>();
            if(CalculateWeight()+meatScript.weight < MAX_WEIGHT) collected.Add(meat);
            else break;
        }

        foreach(var meat in collected) corpse.Inventory.Remove(meat);

        Inventory.AddRange(collected);
    }
    protected override void OnTriggerEnter2D(Collider2D collided)
    /*NB В будущем надо будет отвязать события потребностей от коллайдера*/
    {

        if (_currentState == STATE.SEEK_FOR_FOOD) //NB yнемного оптимизировал!
        {
            if (gender == FEMALE)
            {
                if (collided.tag == "Tree")
                {
                    CollectFruitsFromTree(collided.gameObject);
                }
            }


            else
            {
                foreach (var collidedAnimal in huntableAnimals)
                {
                    if (collided.tag == collidedAnimal.tag)
                    {
                        Kill(collided.gameObject.GetComponent<Creature>());
                    }
                }

                //NB пока сделано не очень, могут быть серьезные баги
                if(collided.tag == "AnimalCorpse")
                {
                    var corpseScript = collided.gameObject.GetComponent<Corpse>();
                    if(corpseScript.currentState == CORPSE_STATE.DEAD) CollectMeatFromCorpse(corpseScript);
                }
            }
        }

        if (collided.tag == tag && _currentState == STATE.SEEK_FOR_PARTNER) //если это человек...
        {

            if (CanMarry(collided.gameObject))
            {
                Marry(collided.gameObject);
            }

            if (collided.gameObject == spouse)
            {
                _sexNecessity.Increase();
                if (gender == MALE)
                {
                    /*NB Спавн детей происходит только если объект мужского пола.
                    Связано это с тем, что коллизия происходит и  у самки, и у самца, и если этого условия не будет,
                    то сгенерируются два ребенка. Возможно, это костыль*/
                    var kid = Instantiate(child, transform.position, Quaternion.identity);


                    var kidScript = kid.GetComponent<Human>(); //Какого-то черта потомки создаются тоже с супругами....
                    kidScript.spouse = null;
                }
            }

        }

    }


    //////////////////Файндеры////////////////////////

    private GameObject[] FindAnimalsToHunt()
    {
        List<GameObject> animalHuntList = new List<GameObject>();
        foreach (GameObject animal_type in huntableAnimals)
        {
            GameObject[] food = GameObject.FindGameObjectsWithTag(animal_type.tag);
            if (!(food.Length < 1))
            {
                animalHuntList.AddRange(food);
            }
        }
        if (animalHuntList.Count < 1) return null;
        else return animalHuntList.ToArray();
    }

    private GameObject[] FindTrees()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        List<GameObject> tmp = new List<GameObject>(); //Временный список...
        foreach (var tree in trees)
        {
            var treeScript = tree.GetComponent<Tree>();
            if (treeScript.Inventory.Count > 0)//Если у дерева есть плоды...
            {
                tmp.Add(tree);//Ура!
            }
        }
        trees = tmp.ToArray();
        if (trees.Length < 1) return null;
        return trees;
    }
    private GameObject[] FindFoodInInventory()
    {
        List<GameObject> foodList = new List<GameObject>();
        foreach (var thing in Inventory)
        {
            if (thing.GetComponent<IEdible>() != null) foodList.Add(thing);
        }

        if (foodList.Count > 0) return foodList.ToArray();
        else return null;
    }
    protected override GameObject[] FindPartners()
    {
        GameObject[] partners = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> tmp = new List<GameObject>(); //Временный список...
        foreach (var partner in partners)
        {
            var partnerScript = partner.GetComponent<Human>();
            if (partnerScript.gender != gender && partnerScript.spouse == null)//Если пол партнера не сопадает с твоим, и у него нет супруга..
            {
                tmp.Add(partner);//Ура!
            }
        }
        partners = tmp.ToArray();
        if (partners.Length < 1) return null;
        return partners;
    }
    protected GameObject[] FindHomes()
    {
        GameObject[] homes = GameObject.FindGameObjectsWithTag("House"); //NB когда появятся другие постройки, переделай
        if (homes.Length < 1) return null;
        return homes;
    }

    //////////////////Потребности////////////////////////

    protected override void UpdateNecessities(bool ready)
    {
        base.UpdateNecessities(ready);

        if (ready) _homeNecessity.Decrease();
    }

    //////////////////Изменение состояния////////////////////////

    void Hunt()
    {
        var animalsList = FindAnimalsToHunt();

        if (animalsList != null)
        {
            Taxis(FindNearest(animalsList));
        }

        else _currentState = STATE.CHILL;

    }

    void SeekForFruits()
    {

        var treesList = FindTrees();

        if (treesList != null)
        {

            Taxis(FindNearest(treesList));
        }

        else _currentState = STATE.CHILL;
    }
    protected override void HandleState(bool ready)
    { /*В зависимости от состояния изменяется логика, по которой существо куда-то идет*/
        if (!ready) return;

        switch (_currentState)
        {
            case STATE.CHILL:
                Wobble();
                break;

            case STATE.SEEK_FOR_FOOD:

                var foodInInventory = FindFoodInInventory();
                if (foodInInventory != null)
                {
                    EatFoodFromInventory(foodInInventory);
                    break;
                }


                if (gender == FEMALE) SeekForFruits();
                else Hunt();

                break;

            case STATE.SEEK_FOR_PARTNER:
                if (spouse != null)
                {
                    Taxis(spouse);
                    //Debug.Log("Партнер есть, поэтому иду к нему");
                }

                else
                {
                    var potentialPartners = FindPartners();
                    if (potentialPartners != null) Taxis(FindNearest(potentialPartners));
                    else _currentState = STATE.CHILL;
                    //Debug.Log("Партнера нет, иду к кому есть");
                }
                break;

            case STATE.SEEK_FOR_HOMEPLACE:
                var homes = FindHomes();
                if (homes != null)
                {
                    var nearest = FindNearest(homes);

                    if (Vector3.Distance(transform.position, nearest) < 1f && Vector3.Distance(transform.position, nearest) > 0.7f) //NB Магические числа!
                    {
                        BuildHome();
                        _homeNecessity.Increase();
                        _homeNecessity.locked = true;
                    }

                    else if (Vector3.Distance(transform.position, nearest) < 0.7f) Antitaxis(nearest);

                    else Taxis(nearest);
                }

                else
                {
                    BuildHome();
                    _homeNecessity.Increase();
                    _homeNecessity.locked = true;
                }

                break;
        }

    }
}
