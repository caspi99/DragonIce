using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum checkboxesId
{
    SETTINGS,
    LEVEL1,
    LEVEL1_SYNC,
    LEVEL1_ASYNC,
    LEVEL2
}

public enum propertiesId
{
    SETTINGS,
    LEVEL1,
    LEVEL2
}

public sealed class GlobalProperties
{
    //GroupOfCheckboxes definition
    private List<System.Reflection.PropertyInfo[]> properties = new List<System.Reflection.PropertyInfo[]>();

    private static GlobalProperties instance = null;
    private GlobalProperties()
    {
        //Settings properties
        properties.Add(typeof(Settings).GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.FlattenHierarchy
            ));

        //Level1 properties
        properties.Add(typeof(CheckboxManager).GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.FlattenHierarchy
            ));

        //Level2 properties
        properties.Add(typeof(CheckboxManagerLevel2).GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.FlattenHierarchy
            ));
    }

    public static GlobalProperties Instance
    {
        get
        {
            if (instance == null)
                instance = new GlobalProperties();
            return instance;
        }
    }

    public System.Reflection.PropertyInfo GetProperty(propertiesId id, string name)
    {
        foreach(System.Reflection.PropertyInfo prop in properties[(int)id])
        {
            if (prop.Name.Equals(name)) { return prop; }
        }

        return null;
    }

    public System.Reflection.PropertyInfo GetProperty(checkboxesId cid, string name)
    {
        // Id process
        propertiesId id = propertiesId.LEVEL1;
        if (cid == checkboxesId.SETTINGS) { id = propertiesId.SETTINGS; }
        if (cid == checkboxesId.LEVEL2) { id = propertiesId.LEVEL2; }

        // We call the original method
        return GetProperty(id, name);
    }
}

public enum language
{
    ENGLISH,
    SPANISH
}

public class TextInfoProperty
{
    public language language_ { get; private set; }
    public string sentence { get; private set; }
    public string additionalInfo { get; private set; }
    public TextInfoProperty(language language_, string sentence) { this.language_ = language_; this.sentence = sentence; }
    public TextInfoProperty(language language_, string sentence, string additionalInfo) { this.language_ = language_; this.sentence = sentence; this.additionalInfo = additionalInfo; }
}

public class PropertyExtended
{
    public System.Reflection.PropertyInfo property { get; private set; }
    private List<TextInfoProperty> infoProperties { get; set; }
    protected PropertyExtended(string property, propertiesId id)
    {
        this.property = GlobalProperties.Instance.GetProperty(id, property);

        this.infoProperties = new List<TextInfoProperty>();
        this.infoProperties.Add(new TextInfoProperty(language.ENGLISH, property));
        this.infoProperties.Add(new TextInfoProperty(language.SPANISH, property));
    }
    protected PropertyExtended(string property, List<TextInfoProperty> infoProperties, propertiesId id)
    {
        this.property = GlobalProperties.Instance.GetProperty(id, property); this.infoProperties = infoProperties;
    }
    public string GetSentence() { return infoProperties.Find(x => x.language_ == CheckboxInterfaceCreator.selectedLanguage).sentence; }
    public string GetInfo() { return infoProperties.Find(x => x.language_ == CheckboxInterfaceCreator.selectedLanguage).additionalInfo; }
}

public class SettingsPropertyExtended : PropertyExtended
{
    public SettingsPropertyExtended(string property):base(property,propertiesId.SETTINGS) { }
    public SettingsPropertyExtended(string property, List<TextInfoProperty> infoProperties) : base(property, infoProperties, propertiesId.SETTINGS) { }
}

public class Level1PropertyExtended : PropertyExtended
{
    public Level1PropertyExtended(string property) : base(property, propertiesId.LEVEL1) { }
    public Level1PropertyExtended(string property, List<TextInfoProperty> infoProperties) : base(property, infoProperties, propertiesId.LEVEL1) { }
}

public class Level2PropertyExtended : PropertyExtended
{
    public Level2PropertyExtended(string property) : base(property, propertiesId.LEVEL2) { }
    public Level2PropertyExtended(string property, List<TextInfoProperty> infoProperties) : base(property, infoProperties, propertiesId.LEVEL2) { }
}

public class GroupOfProperties
{
    private List<TextInfoProperty> title { get; set; }
    public List<PropertyExtended> properties { get; private set; }
    public bool advanced { get; private set; }
    public GroupOfProperties(List<TextInfoProperty> title, List<PropertyExtended> properties, bool advanced = false)
    {
        this.title = title; this.properties = properties; this.advanced = advanced;
    }
    public int GetSizeOfGroup()
    {
        int result = 20; //offset

        foreach(PropertyExtended prop in properties)
        {
            if (string.IsNullOrEmpty(prop.GetInfo())) { result += 70*((int)(prop.GetSentence().Length/109)+1); } else { result += 200; }
        }
        return result;
    }
    public string GetTitle() { return title.Find(x => x.language_ == CheckboxInterfaceCreator.selectedLanguage).sentence; }
    public string GetTitle(language lang) { return title.Find(x => x.language_ == lang).sentence; }
}

public class GroupOfCheckboxes
{
    public int id { get; private set; }
    private List<TextInfoProperty> title { get; set; }
    public string filePath { get; private set; }
    public List<GroupOfProperties> gProperties { get; private set; }
    public int numOfAdvancedSettings { get; private set; }
    public GroupOfCheckboxes(int id, List<TextInfoProperty> title, string filePath, List<GroupOfProperties> gProperties)
    {
        this.id = id; this.title = title; this.filePath = filePath; this.gProperties = gProperties;

        this.numOfAdvancedSettings = GetNumberOfAdvancedSettings();
    }
    public bool Contains(string input)
    {
        foreach(GroupOfProperties group in gProperties)
        {
            foreach(PropertyExtended prop in group.properties)
            {
                if (prop.property.Name.Equals(input)) { return true; }
            }
        }
        return false;
    }
    private int GetNumberOfAdvancedSettings()
    {
        int count = 0;
        foreach(GroupOfProperties gProp in gProperties)
        {
            if (gProp.advanced) { count++; }
        }
        return count;
    }
    public string GetTitle() { return title.Find(x => x.language_ == CheckboxInterfaceCreator.selectedLanguage).sentence; }
    public string GetTitle(language lang) { return title.Find(x => x.language_ == lang).sentence; }
}

public sealed class GlobalGroupsOfCheckboxes
{
    public const bool GENERAL = false;
    public const bool ADVANCED = true;

    //GroupOfCheckboxes definition
    public List<GroupOfCheckboxes> groupsOfCheckboxes = new List<GroupOfCheckboxes>();

    private static GlobalGroupsOfCheckboxes instance = null;
    private GlobalGroupsOfCheckboxes()
    {
        // Get the parent directory of the application and create the CheckboxesConfig folder if it doesn't exist
        string parentDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
        string checkboxesConfigPath = Path.Combine(parentDirectory, "Settings");

        if (!Directory.Exists(checkboxesConfigPath))
        {
            Directory.CreateDirectory(checkboxesConfigPath);
        }

        //SETTINGS
        groupsOfCheckboxes.Add(new GroupOfCheckboxes(
            (int)checkboxesId.SETTINGS,
            //Title
            new List<TextInfoProperty>()
            {
            new TextInfoProperty(language.ENGLISH, "General settings"),
            new TextInfoProperty(language.SPANISH, "Ajustes generales"),
            },
            Path.Combine(checkboxesConfigPath, "Settings.json"),
            new List<GroupOfProperties>() {
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Tracking settings"),
                    new TextInfoProperty(language.SPANISH, "Ajustes del tracking"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new SettingsPropertyExtended("enableTracking", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate the tracking system", "Yes: play with the tracking system; No: play with the keyboard (keys 1-2-3-4 to change the player y W-A-S-D to move the player)"),
                        new TextInfoProperty(language.SPANISH, "Activar el sistema de tracking", "Sí: se jugará con el sistema de tracking; No: se jugará con el teclado (teclas 1-2-3-4 para cambiar el color y W-A-S-D para moverse)."),
                    }),
                    new SettingsPropertyExtended("screenToWorldPosTracking", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate Screen To World tracking (Recommended)", "Yes: The tracking coordinates are converted to screen coordinates and the Camera.ScreenToWorldPoint method is called; No: the tracking coordinates are rescaled and adjusted according to the values of the PluginController script"),
                        new TextInfoProperty(language.SPANISH, "Activar Screen to World tracking (Recomendado)", "Sí: las coordenadas del tracking se convierten en coordenadas de pantalla y se llama al método Camera.ScreenToWorldPoint; No: las coordenadas del tracking se reescalan y ajustan según los valores del script PluginController"),
                    }),
                    new SettingsPropertyExtended("enableRotation", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Use rotation of the tracker with the tracking system"),
                        new TextInfoProperty(language.SPANISH, "Utilizar la rotación del tracker con el sistema de tracking"),
                    }),
                    new SettingsPropertyExtended("enableYAxis", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Use the height of the tracker with the tracking system"),
                        new TextInfoProperty(language.SPANISH, "Utilizar la altura del tracker con el sistema de tracking"),
                    }),
                    new SettingsPropertyExtended("tracking_player_reorder", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate tracking Player Reorder", "Yes: each dock where the player is placed has a specific colour assigned to it; No: each dock where the player is placed does not have a specific colour assigned to it."),
                        new TextInfoProperty(language.SPANISH, "Activar tracking Player Reorder", "Sí: cada muelle donde se coloca el jugador tiene un color asignado específico; No: cada muelle donde se coloca el jugador no tiene asignado un color específico."),
                    }),
                    new SettingsPropertyExtended("light_docks", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Illuminate the docks of the assigned colour before starting the game."),
                        new TextInfoProperty(language.SPANISH, "Iluminar los muelles del color asignado antes de empezar el juego"),
                    })
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Selection of the experience parts to play"),
                    new TextInfoProperty(language.SPANISH, "Selección de las partes a jugar de la experiencia"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new SettingsPropertyExtended("play_level_1", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Play the first part of the experience (breaking the ice)"),
                        new TextInfoProperty(language.SPANISH, "Jugar la primera parte de la experiencia (romper el hielo)"),
                    }),
                    new SettingsPropertyExtended("play_level_2", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Play the second part of the experience (cooperative game)"),
                        new TextInfoProperty(language.SPANISH, "Jugar la segunda parte de la experiencia (juego cooperativo)"),
                    })
                }),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Mode of play for the first part (breaking the ice) "),
                    new TextInfoProperty(language.SPANISH, "Modalidad de juego para la primera parte (romper el hielo)"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new SettingsPropertyExtended("sync", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Select game mode for the first part", "Synchronous: all players step on the stones at the same time; Asynchronous: players step on the stones at different times"),
                        new TextInfoProperty(language.SPANISH, "Selecciona la modalidad de juego para la primera parte", "Síncrona: todos los jugadores pisan las piedras al mismo tiempo; Asíncrona: los jugadores pisan las piedras en tiempos distintos"),
                    })
                }),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Data extraction"),
                    new TextInfoProperty(language.SPANISH, "Extracción de datos"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new SettingsPropertyExtended("enableDataExtraction", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Save the data of the positions and mechanics during the game"),
                        new TextInfoProperty(language.SPANISH, "Guardar los datos de las posiciones y las mecánicas durante la partida"),
                    }),
                    new SettingsPropertyExtended("continuous_data_saving", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Save data continuously (Recommended)", "If selected Yes, it will create the files and write them while executing the game."),
                        new TextInfoProperty(language.SPANISH, "Guardar los datos de manera contínua (Recomendado)", "Al seleccionar Sí, creará los ficheros y los escribirá mientras ejecuta el juego."),
                    }),
                    new SettingsPropertyExtended("update_data_time", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Time interval for recording positions", "Specify the time interval in seconds."),
                        new TextInfoProperty(language.SPANISH, "Intervalo de tiempo para registrar las posiciones", "Especifique el intervalo de tiempo en segundos."),
                    })
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Sound settings"),
                    new TextInfoProperty(language.SPANISH, "Ajustes del sonido"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new SettingsPropertyExtended("binauralSound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Listen the sound binaurally"),
                        new TextInfoProperty(language.SPANISH, "Escuchar el sonido de forma binaural"),
                    })
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Players settings"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de los jugadores"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new SettingsPropertyExtended("visualizePlayerPosition", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Visualize the position of the player with a square of his/her color in the game"),
                        new TextInfoProperty(language.SPANISH, "Visualizar la posición del jugador o jugadora con un cuadrado de su color en el juego"),
                    })
                })
            }));

        //LEVEL 1 Checkboxes
        groupsOfCheckboxes.Add(new GroupOfCheckboxes(
            (int)checkboxesId.LEVEL1,
            //Title
            new List<TextInfoProperty>()
            {
            new TextInfoProperty(language.ENGLISH, "General Settings of the first part (breaking the ice)"),
            new TextInfoProperty(language.SPANISH, "Ajustes generales de la primera parte (romper el hielo)"),
            },
            Path.Combine(checkboxesConfigPath, "CheckboxManagerLevel1Properties.json"),
            new List<GroupOfProperties>() {
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "What narrative audios do you want to be heard?"),
                    new TextInfoProperty(language.SPANISH, "Qué audios de narrativa quieres escuchar?"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("play_dragon_Introduction_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Appearance of the dragon in the environment"),
                        new TextInfoProperty(language.SPANISH, "Aparición del dragón en el entorno"),
                    }),
                    new Level1PropertyExtended("play_dragon_Fly_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Dragon flying across the lake"),
                        new TextInfoProperty(language.SPANISH, "Dragón volando por el lago"),
                    }),
                    new Level1PropertyExtended("play_character_introduction_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Characters entrance"),
                        new TextInfoProperty(language.SPANISH, "Entrada de los personajes"),
                    }),
                    new Level1PropertyExtended("play_freeze_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Lake freezing"),
                        new TextInfoProperty(language.SPANISH, "Congelación del lago"),
                    }),
                    new Level1PropertyExtended("play_help_dragon_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Help the dragon"),
                        new TextInfoProperty(language.SPANISH, "Ayudar al dragón"),
                    }),
                    new Level1PropertyExtended("play_docks_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Go to the docks"),
                        new TextInfoProperty(language.SPANISH, "Ir a los muelles"),
                    }),
                    new Level1PropertyExtended("play_First_Stone_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "First Stone (familiarization phase)"),
                        new TextInfoProperty(language.SPANISH, "Primera piedra (fase familiarización)"),
                    }),
                    new Level1PropertyExtended("play_Adelante_Stone_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Incentive to step on the first stone (familiarization phase)"),
                        new TextInfoProperty(language.SPANISH, "Incentivo para pisar la primera piedra (fase familiarización)"),
                    }),
                    new Level1PropertyExtended("play_Follow_Stone_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Keep stepping on the stones (start of the main game)"),
                        new TextInfoProperty(language.SPANISH, "Seguir pisando las piedras (inicio del juego principal)"),
                    }),
                    new Level1PropertyExtended("play_Final_Narrative", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Back to the docks (end of the game)"),
                        new TextInfoProperty(language.SPANISH, "Volver a los muelles (final del juego)"),
                    })
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the sequences/parts of the experience"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de las secuencias/partes de la experiencia"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("skip_story_sequences", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Skip the narrated introduction of the first part"),
                        new TextInfoProperty(language.SPANISH, "Saltar la introducción narrada de la primera parte"),
                    }),
                    new Level1PropertyExtended("demoTutorialSequence", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Demonstration of how to play at the beginning", "With keyword \"9\" you can throw a stone, and with keyword \"0\" you can activate the stone."),
                        new TextInfoProperty(language.SPANISH, "Hacer una demostración de cómo jugar al inicio", "Con la tecla \"9\" puedes lanzar una piedra, y con la tecla \"0\" puedes activar la piedra."),
                    })
                }),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the animations"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de las animaciones"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("ingame_stay_dragon", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate that the dragon is always in the centre throwing the stones"),
                        new TextInfoProperty(language.SPANISH, "Activar que el dragón se encuentre siempre en el centro lanzando piedras"),
                    })
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the interaction"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de interacción"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("light_stone_when_ready_to_kick", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Lighting the stone once it reaches its final position"),
                        new TextInfoProperty(language.SPANISH, "Iluminar la piedra una vez llega a su posición final"),
                    }),
                    new Level1PropertyExtended("stone_light_intensity", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "(only if you answered Yes in the previous question) Specify the intensity with which you want the stone to light up once it reaches its final position"),
                        new TextInfoProperty(language.SPANISH, "(sólo si has respondido Sí en la anterior pregunta) Indica la intensidad con la que quieres que se ilumine la piedra una vez llega a su posición final"),
                    }),
                    new Level1PropertyExtended("light_stone_while_going_to_final_position", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Light up the stone as it goes to its final position (throwing)"),
                        new TextInfoProperty(language.SPANISH, "Iluminar la piedra mientras va a su posición final (lanzamiento)"),
                    }),
                    new Level1PropertyExtended("stone_light_intensity_while_going_to_final_position", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "(only if you answered Yes in the previous question) Specify the intensity with which you want the stone to light up while it is thrown"),
                        new TextInfoProperty(language.SPANISH, "(sólo si has respondido Sí en la anterior pregunta) Indica la intensidad con la que quieres que se ilumine la piedra mientras es lanzada"),
                    }),
                    new Level1PropertyExtended("stone_transparency_while_going_to_final_position", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the transparency value while the stone reaches its final position:"),
                        new TextInfoProperty(language.SPANISH, "Indica el valor de transparencia mientras la piedra llega a su posición final:"),
                    }),
                    new Level1PropertyExtended("stone_explosion_intensity", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the intensity with which you want the explosion of the stone to be visualised:"),
                        new TextInfoProperty(language.SPANISH, "Indica la intensidad con la que quieres que se visualice la explosión de la piedra:"),
                    }),
                    new Level1PropertyExtended("light_lake", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Enable the lake to shine when all four players activate the stones"),
                        new TextInfoProperty(language.SPANISH, "Activar que el lago se ilumine cuando los cuatro jugadores activan las piedras"),
                    }),
                    new Level1PropertyExtended("lake_light_intensity", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "(only if you answered Yes to the previous question) Specify the intensity of the illumination of the lake:"),
                        new TextInfoProperty(language.SPANISH, "(sólo si has respondido Sí a la anterior pregunta) Indica la intensidad de la iluminación del lago:"),
                    }),
                    new Level1PropertyExtended("seconds_to_light_lake", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "(only if you answered Yes to the previous questio) Specify the duration (seconds) of the illumination of the lake:"),
                        new TextInfoProperty(language.SPANISH, "(sólo si has respondido Sí a la anterior pregunta) Indica la duración (segundos) de la iluminación del lago:"),
                    })
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Sounds settings"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de los sonidos"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("play_round_pass_sound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate sound to indicate that at least one player has activated a stone"),
                        new TextInfoProperty(language.SPANISH, "Activar sonido para indicar que al menos un jugador ha activado una piedra")
                    }),
                    new Level1PropertyExtended("play_throw_stone_sound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate sound for stone throwing"),
                        new TextInfoProperty(language.SPANISH, "Activar sonido para el lanzamiento de la piedra")
                    }),
                    new Level1PropertyExtended("play_shine_stone_sound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate sound for when the stone reaches the final position"),
                        new TextInfoProperty(language.SPANISH, "Activar sonido para cuando la piedra llega a la posición final")
                    }),
                    new Level1PropertyExtended("play_kick_stone_sound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate sound for when the stone is stepped on (stone activation)"),
                        new TextInfoProperty(language.SPANISH, "Activar sonido para cuando la piedra es pisada (activación de la piedra)")
                    }),
                    new Level1PropertyExtended("play_ice_crack_sound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate sound for crack creation after stepping on the stone"),
                        new TextInfoProperty(language.SPANISH, "Activar sonido para la creación de la grieta después de pisar la piedra")
                    }),
                    new Level1PropertyExtended("onlyKickPerfectSound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate same sound for when stones are activated during and after the 'perfect time'"),
                        new TextInfoProperty(language.SPANISH, "Activar mismo sonido para cuando las piedras son activadas durante y después el 'tiempo perfecto'")
                    }),
                    new Level1PropertyExtended("muteGoodSound", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Mute sound for when stones are stepped on at a time other than 'perfect time'"),
                        new TextInfoProperty(language.SPANISH, "Silenciar sonido para cuando las piedras son pisadas en otro momento que no sea el 'tiempo perfecto'")
                    }),
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the particles when activating the stone"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de las partículas al activar la piedra"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("perfectTimingParticles", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate a different effect (explosion) when the stone is stepped on in the 'perfect time'."),
                        new TextInfoProperty(language.SPANISH, "Activar efecto distinto (explosión) cuando la piedra es pisada en el 'tiempo perfecto'"),
                    }),
                    new Level1PropertyExtended("seconds_to_perfect_timing", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "(answer only if Yes has been selected in the previous question) The time to step in 'perfect time' (once the stone reaches its final position) will be (in seconds):"),
                        new TextInfoProperty(language.SPANISH, "(responder sólo si se ha indicado Sí en la anterior pregunta) El tiempo para pisar en el “tiempo perfecto” (una vez la piedra llega a su posición final) será de (en segundos):"),
                    }),
                    new Level1PropertyExtended("particleSpeed", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the particle speed during the explosion effect:"),
                        new TextInfoProperty(language.SPANISH, "Indica la velocidad de las partículas durante el efecto de explosión:"),
                    }),
                    new Level1PropertyExtended("particleMovingTime", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the movement time of the particles during the explosion:"),
                        new TextInfoProperty(language.SPANISH, "Indica el tiempo de movimiento de las partículas durante la explosión:"),
                    }),
                    new Level1PropertyExtended("particleDestroyTime", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the lifetime of the particles during the explosion:"),
                        new TextInfoProperty(language.SPANISH, "Indica el tiempo de vida de las partículas durante la explosión:"),
                    }),
                    new Level1PropertyExtended("particleRateRange", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the density range of the explosion particles:"),
                        new TextInfoProperty(language.SPANISH, "Indica el rango de densidad de las partículas de la explosión:"),
                    }),
                    new Level1PropertyExtended("particleSizeRange", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the size range of the particles in the explosion:"),
                        new TextInfoProperty(language.SPANISH, "Indica el rango de tamaño de las partículas de la explosión:"),
                    }),
                    new Level1PropertyExtended("brightParticles", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate particles shining during the explosion:"),
                        new TextInfoProperty(language.SPANISH, "Activar que las partículas se iluminen durante la explosión:"),
                    })
                },
                ADVANCED),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the training phase"),
                    new TextInfoProperty(language.SPANISH, "Ajustes fase de entrenamiento"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("enableTrainingRounds", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate the training phase"),
                        new TextInfoProperty(language.SPANISH, "Activa la fase de entrenamiento"),
                    }),
                    new Level1PropertyExtended("number_of_training_rounds", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the number of stones thrown per player during the training phase:"),
                        new TextInfoProperty(language.SPANISH, "Indica el número de piedras lanzadas por jugador/a durante la fase de entrenamiento:"),
                    }),
                    new Level1PropertyExtended("trainingAlwaysWithTheSameAngle", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activates that the throwing angle for the stones during the training phase is always the same."),
                        new TextInfoProperty(language.SPANISH, "Activa que el ángulo de lanzamiento para las piedras durante la fase de entrenamiento sea siempre el mismo"),
                    }),
                    new Level1PropertyExtended("firstStonesNotDestroyed", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activates that the stones do not disappear during the training phase until they are activated"),
                        new TextInfoProperty(language.SPANISH, "Activa que las piedras no desaparezcan durante la fase de entrenamiento hasta que no son activadas"),
                    }),
                    new Level1PropertyExtended("numberOfStonesNotDestroyed", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "(answer only if you have selected Yes in the previous question) Specify the number of stones that you do not want to make disappear:", "If you indicate a number greater than the number of training rounds, the adjustment will be applied to the remaining number of stones in the familiarization phase"),
                        new TextInfoProperty(language.SPANISH, "(responder sólo si se ha indicado Sí en la anterior pregunta) Indica el número de piedras que quieres que no desaparezcan:", "En caso de indicar un número mayor que el número de rondas de entrenamiento, se aplicará el ajuste al número sobrante de piedras en la fase de familiarización"),
                    }),
                    new Level1PropertyExtended("time_between_previous_stone_training", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the waiting time between throws, when pressing the T key, during the training phase:"),
                        new TextInfoProperty(language.SPANISH, "Indica el tiempo de espera entre lanzamientos, al apretar la tecla T, durante la fase de entrenamiento:"),
                    }),
                    new Level1PropertyExtended("stone_speed_training", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the speed of the stone while reaching its final position:"),
                        new TextInfoProperty(language.SPANISH, "Indica la velocidad de la piedra mientras llega a su posición final:"),
                    }),
                    new Level1PropertyExtended("seconds_to_kick_training", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "(in case you have activated that the stones disappear) Specify the time to step on the stones during the training phase:"),
                        new TextInfoProperty(language.SPANISH, "(en caso de haber activado que las piedras desaparezcan) Indica el tiempo para pisar las piedras durante la fase de entrenamiento:"),
                    })
                }),
        }));
        //LEVEL 1 SYNC Checkboxes
        groupsOfCheckboxes.Add(
            new GroupOfCheckboxes(
                (int)checkboxesId.LEVEL1_SYNC,
                //Title
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the first part: synchronous version"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de la primera parte: versión en sincronía"),
                },
                Path.Combine(checkboxesConfigPath, "CheckboxManagerLevel1SyncProperties.json"),
                new List<GroupOfProperties>() {
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "General settings of the stone mechanics"),
                        new TextInfoProperty(language.SPANISH, "Ajustes generales de la mecánica de la piedra"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level1PropertyExtended("distance_to_target", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the maximum distance between the stone and the centre:"),
                            new TextInfoProperty(language.SPANISH, "Indica la distancia máxima entre la piedra y el centro:"),
                        }),
                        new Level1PropertyExtended("minimum_distance_to_target", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the minimum distance between the stone and the centre:"),
                            new TextInfoProperty(language.SPANISH, "Indica la distancia mínima entre la piedra y el centro:"),
                        }),
                        new Level1PropertyExtended("angle_addition_interval", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the angular distance between the final positions of the stones:"),
                            new TextInfoProperty(language.SPANISH, "Indica la distancia angular entre las posiciones finales de las piedras:"),
                        })
                    }),
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Settings of the familiarization phase"),
                        new TextInfoProperty(language.SPANISH, "Ajustes de la fase de familiarización"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level1PropertyExtended("number_of_tutorial_rounds", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the number of stones you want to be thrown per player during the familiarization phase:"),
                            new TextInfoProperty(language.SPANISH, "Indica el número de piedras que quieres que se lancen por jugador durante la fase de familiarización:"),
                        }),
                        new Level1PropertyExtended("time_between_previous_stone_tutorial_interval", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the waiting time between stone throws (in seconds):"),
                            new TextInfoProperty(language.SPANISH, "Indica el tiempo de espera entre los lanzamientos de las piedras (en segundos):"),
                        }),
                        new Level1PropertyExtended("wait_until_famirialitzation_ends", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate not to start the main phase before the entire familiarization phase has been finished"),
                            new TextInfoProperty(language.SPANISH, "Activar no empezar la fase principal sin que haya terminado toda la fase de familiarización"),
                        }),
                        new Level1PropertyExtended("stone_speed_tutorial_interval", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the speed at which you want the stone to reach its final position"),
                            new TextInfoProperty(language.SPANISH, "Indica la velocidad con la que quieres que la piedra llegue a su posición final"),
                        }),
                        new Level1PropertyExtended("seconds_to_kick_tutorial", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify for how long (seconds) you want the stone to be visible to be stepped on"),
                            new TextInfoProperty(language.SPANISH, "Indica durante cuánto tiempo (segundos) quieres que la piedra sea visible para ser pisada"),
                        })
                    }),
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Settings of the principal phase"),
                        new TextInfoProperty(language.SPANISH, "Ajustes de la fase principal"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level1PropertyExtended("number_of_true_rounds", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the number of stones you want to be thrown per player during the main phase:"),
                            new TextInfoProperty(language.SPANISH, "Indica el número de piedras que quieres que se lancen por jugador durante la fase principal:"),
                        }),
                        new Level1PropertyExtended("time_between_previous_stone_true_interval", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the waiting time between stone throws (in seconds):"),
                            new TextInfoProperty(language.SPANISH, "Indica el tiempo de espera entre los lanzamientos de las piedras (en segundos):"),
                        }),
                        new Level1PropertyExtended("stone_speed_true_interval", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the speed at which you want the stone to reach its final position:"),
                            new TextInfoProperty(language.SPANISH, "Indica la velocidad con la que quieres que la piedra llegue a su posición final:"),
                        }),
                        new Level1PropertyExtended("seconds_to_kick_true", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify for how long (seconds) you want the stone to be visible to be stepped on:"),
                            new TextInfoProperty(language.SPANISH, "Indica durante cuánto tiempo (segundos) quieres que la piedra sea visible para ser pisada"),
                        })
                    })
                }));

    //LEVEL 1 ASYNC Checkboxes
    groupsOfCheckboxes.Add(new GroupOfCheckboxes(
        (int)checkboxesId.LEVEL1_ASYNC,
        //Title
        new List<TextInfoProperty>()
        {
            new TextInfoProperty(language.ENGLISH, "Settings of the first part: asynchronous version"),
            new TextInfoProperty(language.SPANISH, "Ajustes de la primera parte: versión en asincronía")
        },
        Path.Combine(checkboxesConfigPath, "CheckboxManagerLevel1AsyncProperties.json"),
        new List<GroupOfProperties>() {
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "General settings of the stone mechanics"),
                    new TextInfoProperty(language.SPANISH, "Ajustes generales de la mecánica de la piedra"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("distance_to_target", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Indicates the maximum distance between the stone and the centre:"),
                        new TextInfoProperty(language.SPANISH, "Indica la máxima distancia entre la piedra y el centro:"),
                    }),
                    new Level1PropertyExtended("minimum_distance_to_target", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the minimum distance between the stone and the centre:"),
                        new TextInfoProperty(language.SPANISH, "Indica la distancia mínima entre la piedra y el centro:"),
                    }),
                    new Level1PropertyExtended("angle_addition_interval", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the angular distance between the final positions of the stones:"),
                        new TextInfoProperty(language.SPANISH, "Indica la distancia angular entre las posiciones finales de las piedras:"),
                    }),
                    new Level1PropertyExtended("fixedDistanceToCenter", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate that the distances between the final position of the stone and the centre are fixed."),
                        new TextInfoProperty(language.SPANISH, "Activar que las distancias entre la posición final de la piedra y el centro sean fijas"),
                    }),
                    new Level1PropertyExtended("fixedDistancesToCenter", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Distances between the final position of the stone and the centre for each player:"),
                        new TextInfoProperty(language.SPANISH, "Distancias entre la posición final de la piedra y el centro para cada jugador:"),
                    })
                }),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the familiarization phase"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de la fase de familiarización"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("number_of_tutorial_rounds", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the number of stones you want to be thrown per player during the familiarization phase:"),
                        new TextInfoProperty(language.SPANISH, "Indica el número de piedras que quieres que se lancen por jugador durante la fase de familiarización:"),
                    }),
                    new Level1PropertyExtended("time_between_previous_stone_tutorial_interval", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the waiting time between stone throws (in seconds):"),
                        new TextInfoProperty(language.SPANISH, "Indica el tiempo de espera entre los lanzamientos de las piedras (en segundos):"),
                    }),
                    new Level1PropertyExtended("wait_until_famirialitzation_ends", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Activate not to start the main phase before the entire familiarization phase has been finished."),
                        new TextInfoProperty(language.SPANISH, "Activar no empezar la fase principal sin que haya terminado toda la fase de familiarización"),
                    }),
                    new Level1PropertyExtended("stone_speed_tutorial_interval", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the speed at which you want the stone to reach its final position."),
                        new TextInfoProperty(language.SPANISH, "Indica la velocidad con la que quieres que la piedra llegue a su posición final"),
                    }),
                    new Level1PropertyExtended("seconds_to_kick_tutorial", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify for how long (seconds) you want the stone to be visible to be stepped on."),
                        new TextInfoProperty(language.SPANISH, "Indica durante cuánto tiempo (segundos) quieres que la piedra sea visible para ser pisada"),
                    }),
                }),
            new GroupOfProperties(
                //Subtitle
                new List<TextInfoProperty>()
                {
                    new TextInfoProperty(language.ENGLISH, "Settings of the principal phase"),
                    new TextInfoProperty(language.SPANISH, "Ajustes de la fase principal"),
                },
                //Variables
                new List<PropertyExtended>()
                {
                    new Level1PropertyExtended("number_of_true_rounds", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the number of stones you want to be thrown per player during the main phase:"),
                        new TextInfoProperty(language.SPANISH, "Indica el número de piedras que quieres que se lancen por jugador durante la fase principal:"),
                    }),
                    new Level1PropertyExtended("time_between_previous_stone_true_interval", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the waiting time between stone throws (in seconds):"),
                        new TextInfoProperty(language.SPANISH, "Indica el tiempo de espera entre los lanzamientos de las piedras (en segundos):"),
                    }),
                    new Level1PropertyExtended("stone_speed_true_interval", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify the speed at which you want the stone to reach its final position."),
                        new TextInfoProperty(language.SPANISH, "Indica la velocidad con la que quieres que la piedra llegue a su posición final"),
                    }),
                    new Level1PropertyExtended("seconds_to_kick_true", new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Specify for how long (seconds) you want the stone to be visible to be stepped on."),
                        new TextInfoProperty(language.SPANISH, "Indica durante cuánto tiempo (segundos) quieres que la piedra sea visible para ser pisada"),
                    }),
                })
        }));

        //LEVEL 2 Checkboxes
        groupsOfCheckboxes.Add(new GroupOfCheckboxes(
            (int)checkboxesId.LEVEL2,
            //Title
            new List<TextInfoProperty>()
            {
                new TextInfoProperty(language.ENGLISH, "Settings of the second part"),
                new TextInfoProperty(language.SPANISH, "Ajustes de la segunda parte"),
            },
            Path.Combine(checkboxesConfigPath, "CheckboxManagerLevel2Properties.json"),
            new List<GroupOfProperties>() {
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Settings of the sequence/parts"),
                        new TextInfoProperty(language.SPANISH, "Ajustes de las secuencias/partes"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level2PropertyExtended("skip_story_sequences", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Skip narrated introduction of the second part"),
                            new TextInfoProperty(language.SPANISH, "Saltar la introducción narrada de la segunda parte"),
                        })
                    }),
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Settings of the sequence/parts"),
                        new TextInfoProperty(language.SPANISH, "Ajustes de las secuencias/partes"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level2PropertyExtended("shorterStorySequences", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate shorter transitions between the different phases"),
                            new TextInfoProperty(language.SPANISH, "Activar que las transiciones entre las diferentes fases sean más cortas"),
                        })
                    },
                    ADVANCED),
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Settings of the phases"),
                        new TextInfoProperty(language.SPANISH, "Ajustes de las fases"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level2PropertyExtended("num_of_stones_phase1", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "PHASE 1: Specify the total number of stone slabs to be incorporated in the road (maximum 24)", "It is recommended to be multiple of 4"),
                            new TextInfoProperty(language.SPANISH, "FASE 1: Indica la cantidad total de losas de piedra que deben incorporarse en el camino (máximo 24)", "Se recomienda que sea múltiple de 4"),
                        }),
                        new Level2PropertyExtended("num_of_trees_phase2", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "PHASE 2: Specify the total number of logs to be incorporated in the bridge (maximum 24)", "It is recommended to be multiple of 4"),
                            new TextInfoProperty(language.SPANISH, "FASE 2: Indica la cantidad total de troncos que deben incorporarse en el puente (máximo 24)", "Se recomienda que sea múltiple de 4"),
                        }),
                        new Level2PropertyExtended("num_of_stones_phase3", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "PHASE 3:", "Keep it to 0"),
                            new TextInfoProperty(language.SPANISH, "FASE 3:", "Dejar a 0"),
                        })
                    }),
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Interaction"),
                        new TextInfoProperty(language.SPANISH, "Ajustes de la interacción"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level2PropertyExtended("exclusivePlayStyleGrabbables", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate enforced collaboration"),
                            new TextInfoProperty(language.SPANISH, "Activa la colaboración forzada"),
                        }),
                        new Level2PropertyExtended("grabbable_move_speed_solo", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the speed to move the stone or log individually:"),
                            new TextInfoProperty(language.SPANISH, "Indica la velocidad para mover la piedra o el tronco individualmente:"),
                        }),
                        new Level2PropertyExtended("use_rigidbody_translation", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Use the Unity physics system to calculate friction (Recommended)"),
                            new TextInfoProperty(language.SPANISH, "Utilizar sistema de físicas de Unity para calcular la fricción (Recomendado)"),
                        }),
                        new Level2PropertyExtended("inclinate_grabbable", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate the inclination effect when grabbing a stone or log from the side."),
                            new TextInfoProperty(language.SPANISH, "Activar efecto de inclinación al coger una piedra o tronco por el lateral"),
                        }),
                        new Level2PropertyExtended("show_grips", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Show the grips on the stone or log"),
                            new TextInfoProperty(language.SPANISH, "Mostrar los agarres (grips) de la piedra o tronco"),
                        })
                    },
                    ADVANCED),
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Settings to incorportate the stones (first phase)"),
                        new TextInfoProperty(language.SPANISH, "Ajustes para incorporar las piedras (primera fase)"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level2PropertyExtended("phase_1_check_orientation", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Enables that the stones are incorporated with the specific orientation."),
                            new TextInfoProperty(language.SPANISH, "Activa que las piedras se incorporen con la orientación específica"),
                        }),
                        new Level2PropertyExtended("show_stone1_placement", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Enables the visualization of the final position where the stones should go"),
                            new TextInfoProperty(language.SPANISH, "Activa que se visualice la posición final donde deben ir las piedras"),
                        }),
                        new Level2PropertyExtended("stayToPickStone", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate to wait for a while to grab the stone"),
                            new TextInfoProperty(language.SPANISH, "Activa que haya que esperarse un tiempo para agarrar la piedra"),
                        }),
                        new Level2PropertyExtended("stayToPickStoneTime", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the waiting time for grabbing the stone:"),
                            new TextInfoProperty(language.SPANISH, "Indica el tiempo de espera para agarra la piedra:"),
                        }),
                        new Level2PropertyExtended("allGripsUntachCollaborative", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate that all grips are released if a player detaches during the collaboration"),
                            new TextInfoProperty(language.SPANISH, "Activa que todos los agarres se suelten si un jugador se desengancha durante la colaboración"),
                        }),
                        new Level2PropertyExtended("preferencePlayerStone", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate that the first player who grabs the stone has preference"),
                            new TextInfoProperty(language.SPANISH, "Activa que el primer jugador/a en coger la piedra tenga preferencia"),
                        }),
                        new Level2PropertyExtended("biggerGripsWhenGrabbed", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Activate that grips are larger when grabbing the stone"),
                            new TextInfoProperty(language.SPANISH, "Activa que los agarres (grips) se hagan más grandes al coger la piedra"),
                        })
                    },
                    ADVANCED),
                new GroupOfProperties(
                    //Subtitle
                    new List<TextInfoProperty>()
                    {
                        new TextInfoProperty(language.ENGLISH, "Settings to incorportate the logs (second phase)"),
                        new TextInfoProperty(language.SPANISH, "Ajustes para incorporar los troncos (segunda fase)"),
                    },
                    //Variables
                    new List<PropertyExtended>()
                    {
                        new Level2PropertyExtended("show_table_placement", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Enables the visualization of the final position where the log should go"),
                            new TextInfoProperty(language.SPANISH, "Activa que se visualice la posición final donde deben ir los troncos"),
                        }),
                        new Level2PropertyExtended("pullTreeMechanic", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Enables logs to be pulled"),
                            new TextInfoProperty(language.SPANISH, "Activa que se agarren los troncos y no se empujen"),
                        }),
                        new Level2PropertyExtended("player_mass", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "(if you answered Yes in the previous question) Specify the mass of the player when pushing the log:"),
                            new TextInfoProperty(language.SPANISH, "(si has respondido Sí en la anterior pregunta) Indica la masa del jugador al empujar el tronco:"),
                        }),
                        new Level2PropertyExtended("tree_mass_touching", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "(if you answered Yes to the previous question) Specify the mass of the trunk when touched:"),
                            new TextInfoProperty(language.SPANISH, "(si has respondido Sí en la anterior pregunta) Indica la masa del tronco al ser tocado:"),
                        }),
                        new Level2PropertyExtended("tree_mass_default", new List<TextInfoProperty>()
                        {
                            new TextInfoProperty(language.ENGLISH, "Specify the mass of the log in idle state."),
                            new TextInfoProperty(language.SPANISH, "Indica la masa del tronco en estado de reposo"),
                        })
                    },
                    ADVANCED)
            }));
    }

    public static GlobalGroupsOfCheckboxes Instance
    {
        get
        {
            if (instance == null)
                instance = new GlobalGroupsOfCheckboxes();
            return instance;
        }
    }
}

public class CheckboxInterfaceCreator : MonoBehaviour
{
    public static language selectedLanguage = language.ENGLISH;

    [SerializeField] private Transform canvas; // Canvas
    [SerializeField] private Transform content; // Content window to which UI elements will be added
    [SerializeField] private GameObject emptyUIPrefab;
    [SerializeField] private GameObject titlePrefab;
    [SerializeField] private GameObject subtitlePrefab;
    [SerializeField] private GameObject yesNoQuestionPrefab;
    [SerializeField] private GameObject inputFieldPrefab;
    [SerializeField] private GameObject inputFieldVector2Prefab;
    [SerializeField] private GameObject inputFieldFloat4Prefab;
    [SerializeField] private GameObject buttonPrefab;

    //GroupOfCheckboxes definition
    //private List<GroupOfCheckboxes> groupsOfCheckboxes = new List<GroupOfCheckboxes>();

    // Current groupOfCheckboxes
    private int currGroup = (int)checkboxesId.SETTINGS;

    // Current advanced status
    private bool advancedStatus = false;

    private void Start()
    {
        //InitGroupsOfCheckboxes();
        //LoadAndGenerateUI();
        GenerateUILanguageSelection();
    }

    private GroupOfCheckboxes GetCurrentCheckboxes() { return GlobalGroupsOfCheckboxes.Instance.groupsOfCheckboxes[currGroup]; }

    private void WipeAndCreateUISkeleton(int margins = 50, float spacing = 30f)
    {
        // Add CanvasScaler to the canvas
        CanvasScaler canvasScaler = canvas.gameObject.GetComponent<CanvasScaler>();
        if (canvasScaler == null)
        {
            canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        // You may need to adjust CanvasScaler settings based on your requirements
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);

        // Clear existing content
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        //Adjust the content to the top
        content.position = new Vector2(0, 0);

        // Add a layout group to the content
        VerticalLayoutGroup layoutGroup = content.gameObject.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = content.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.spacing = spacing; // Adjust the spacing between UI elements

        // Add padding to create margins (adjust as needed)
        layoutGroup.padding.top = margins;    // Top margin
        layoutGroup.padding.bottom = margins; // Bottom margin

        // Add more settings to the layout group
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        // Add a content size fitter to the content
        ContentSizeFitter sizeFitter = content.gameObject.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();
        }

        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private RectTransform CreateHorizontalLayout(float spacing = 10f, TextAnchor alignment = TextAnchor.MiddleCenter)
    {
        //Horizontal Layout to add Tittle and Start Button
        RectTransform horizontalLayout = Instantiate(emptyUIPrefab, content).GetComponent<RectTransform>();
        horizontalLayout.sizeDelta = new Vector2(1700, 100);
        HorizontalLayoutGroup horizontalLayoutComponent = horizontalLayout.gameObject.AddComponent<HorizontalLayoutGroup>();

        horizontalLayoutComponent.spacing = spacing; // Adjust the spacing between UI elements

        // Add more settings to the horizontal layout group
        horizontalLayoutComponent.childAlignment = alignment;
        horizontalLayoutComponent.childControlWidth = false;
        horizontalLayoutComponent.childControlHeight = false;
        horizontalLayoutComponent.childForceExpandWidth = false;
        horizontalLayoutComponent.childForceExpandHeight = false;

        return horizontalLayout;
    }

    private void GenerateUILanguageSelection()
    {
        WipeAndCreateUISkeleton(margins:100, spacing:100f);

        RectTransform horizontalLayout = CreateHorizontalLayout(spacing: 30f);

        //ENGLISH BUTTON
        Button englishButtonComponent = Instantiate(buttonPrefab, horizontalLayout).GetComponent<Button>();
        englishButtonComponent.name = "English"; englishButtonComponent.GetComponentInChildren<TextMeshProUGUI>().text = "English";
        englishButtonComponent.onClick.AddListener(() => LanguageButtonFunction(language.ENGLISH));
        //englishButtonComponent.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(640f, 180f);

        //SPANISH BUTTON
        Button spanishButtonComponent = Instantiate(buttonPrefab, horizontalLayout).GetComponent<Button>();
        spanishButtonComponent.name = "Español"; spanishButtonComponent.GetComponentInChildren<TextMeshProUGUI>().text = "Español";
        spanishButtonComponent.onClick.AddListener(() => LanguageButtonFunction(language.SPANISH));
        //spanishButtonComponent.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(640f, 180f);
    }

    private void LanguageButtonFunction(language inputLanguage)
    {
        selectedLanguage = inputLanguage;
        GenerateUIWelcomeScreen();
    }

    private void GenerateUIWelcomeScreen()
    {
        WipeAndCreateUISkeleton(margins: 100, spacing: 250f);

        //Adding the title of the settings group
        string welcomeText = selectedLanguage == language.ENGLISH ? "Welcome to DragonIce Game" : selectedLanguage == language.SPANISH ? "Bienvenido/a al juego DragonIce" : "";
        GameObject title = Instantiate(titlePrefab, content);
        title.GetComponent<TextMeshProUGUI>().text = welcomeText;
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        RectTransform horizontalLayout = CreateHorizontalLayout(spacing: 40f);

        //Start Button
        GameObject startButton = Instantiate(buttonPrefab, horizontalLayout);
        string startText = selectedLanguage == language.ENGLISH ? "Start" : selectedLanguage == language.SPANISH ? "Empezar" : "";
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = startText;
        RectTransform rectTransform = startButton.GetComponent<RectTransform>();
        //rectTransform.sizeDelta = new Vector2(640f, 180f); // Adjust the width and height as needed
        startButton.GetComponent<Button>().onClick.AddListener(() => StartButtonClick(true));

        //Edit Settings Button
        GameObject editSettingsButton = Instantiate(buttonPrefab, horizontalLayout);
        string editSettingsText = selectedLanguage == language.ENGLISH ? "Edit Settings" : selectedLanguage == language.SPANISH ? "Modificar ajustes" : "";
        editSettingsButton.GetComponentInChildren<TextMeshProUGUI>().text = editSettingsText;
        RectTransform rectCTransform = editSettingsButton.GetComponent<RectTransform>();
        //rectCTransform.sizeDelta = new Vector2(640f, 180f); // Adjust the width and height as needed
        editSettingsButton.GetComponent<Button>().onClick.AddListener(LoadAndGenerateUI);

        //Back Button
        RectTransform horizontalLayoutBack = CreateHorizontalLayout(alignment: TextAnchor.MiddleLeft);
        GameObject backButton = Instantiate(buttonPrefab, horizontalLayoutBack);
        string backText = selectedLanguage == language.ENGLISH ? "Back" : selectedLanguage == language.SPANISH ? "Atrás" : "";
        backButton.GetComponentInChildren<TextMeshProUGUI>().text = backText;
        RectTransform backButtonTransform = backButton.GetComponent<RectTransform>();
        //backButtonTransform.sizeDelta = new Vector2(320f, 90f); // Adjust the width and height as needed
        backButton.GetComponent<Button>().onClick.AddListener(GenerateUILanguageSelection);
    }

    private void LoadAndGenerateUI()
    {
        LoadPropertiesFromJson();
        GenerateUI();
    }

    private void GenerateUI()
    {
        WipeAndCreateUISkeleton();
        
        RectTransform horizontalLayout1 = CreateHorizontalLayout();

        //Adding the title of the settings group
        Instantiate(titlePrefab, horizontalLayout1).GetComponent<TextMeshProUGUI>().text = GetCurrentCheckboxes().GetTitle();

        //Advanced Settings Button
        if (advancedStatus == GlobalGroupsOfCheckboxes.GENERAL)
        {
            if (GetCurrentCheckboxes().numOfAdvancedSettings > 0)
            {
                GameObject advancedButton = Instantiate(buttonPrefab, horizontalLayout1);
                string advancedText = selectedLanguage == language.ENGLISH ? "Advanced Settings" : selectedLanguage == language.SPANISH ? "Ajustes avanzados" : "";
                advancedButton.GetComponentInChildren<TextMeshProUGUI>().text = advancedText;
                //advancedButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 40f;
                RectTransform rectADransform = advancedButton.GetComponent<RectTransform>();
                //rectADransform.sizeDelta = new Vector2(320f, 90f); // Adjust the width and height as needed
                advancedButton.GetComponent<Button>().onClick.AddListener(AdvancedSettingsButtonClick);
            }
        }
        //Back Button
        else
        {
            GameObject backButton = Instantiate(buttonPrefab, horizontalLayout1);
            string backText = selectedLanguage == language.ENGLISH ? "Back" : selectedLanguage == language.SPANISH ? "Atrás" : "";
            backButton.GetComponentInChildren<TextMeshProUGUI>().text = backText;
            RectTransform backButtonTransform = backButton.GetComponent<RectTransform>();
            //backButtonTransform.sizeDelta = new Vector2(320f, 90f); // Adjust the width and height as needed
            backButton.GetComponent<Button>().onClick.AddListener(BackButtonClick);
        }

        //Adding the Checkboxes
        foreach (GroupOfProperties gProperties in GetCurrentCheckboxes().gProperties)
        {
            if (gProperties.advanced == advancedStatus)
            {
                //Adding the subtitle of the properties group
                Instantiate(subtitlePrefab, content).GetComponent<TextMeshProUGUI>().text = gProperties.GetTitle();

                //Vertical Layout for the group of variables
                RectTransform groupOfVariablesLayout = Instantiate(emptyUIPrefab, content).GetComponent<RectTransform>();
                groupOfVariablesLayout.sizeDelta = new Vector2(1600, gProperties.GetSizeOfGroup());
                VerticalLayoutGroup groupOfVariablesLayoutComponent = groupOfVariablesLayout.gameObject.AddComponent<VerticalLayoutGroup>();

                // Add more settings to the vertical layout group
                groupOfVariablesLayoutComponent.spacing = 1f; // Adjust the spacing between UI elements
                groupOfVariablesLayoutComponent.padding.top = 0;    // Top margin
                groupOfVariablesLayoutComponent.padding.bottom = 0; // Bottom margin
                groupOfVariablesLayoutComponent.childAlignment = TextAnchor.UpperCenter;
                groupOfVariablesLayoutComponent.childControlWidth = false;
                groupOfVariablesLayoutComponent.childControlHeight = false;
                groupOfVariablesLayoutComponent.childForceExpandWidth = false;
                groupOfVariablesLayoutComponent.childForceExpandHeight = false;

                foreach (PropertyExtended prop in gProperties.properties)
                {
                    CreateUIElement(prop, groupOfVariablesLayout);
                }
            }
        }

        if ((advancedStatus == GlobalGroupsOfCheckboxes.GENERAL))
        {
            RectTransform horizontalLayout2 = CreateHorizontalLayout();

            //Back Button
            GameObject backButton = Instantiate(buttonPrefab, horizontalLayout2);
            string backText = selectedLanguage == language.ENGLISH ? "Back" : selectedLanguage == language.SPANISH ? "Atrás" : "";
            backButton.GetComponentInChildren<TextMeshProUGUI>().text = backText;
            RectTransform backButtonTransform = backButton.GetComponent<RectTransform>();
            //backButtonTransform.sizeDelta = new Vector2(320f, 90f); // Adjust the width and height as needed
            backButton.GetComponent<Button>().onClick.AddListener(BackButtonClick);

            //Continue Button
            GameObject continueButton = Instantiate(buttonPrefab, horizontalLayout2);
            string continueText = selectedLanguage == language.ENGLISH ? "Continue" : selectedLanguage == language.SPANISH ? "Continuar" : "";
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = continueText;
            RectTransform rectCTransform = continueButton.GetComponent<RectTransform>();
            //rectCTransform.sizeDelta = new Vector2(320f, 90f); // Adjust the width and height as needed
            continueButton.GetComponent<Button>().onClick.AddListener(ContinueButtonClick);

            //Start Button
            if (currGroup == (int)checkboxesId.LEVEL2)
            {
                string saveAndStartText = selectedLanguage == language.ENGLISH ? "Save & start" : selectedLanguage == language.SPANISH ? "Guardar y empezar" : "";
                continueButton.GetComponentInChildren<TextMeshProUGUI>().text = saveAndStartText;
                //continueButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 40f;
            }
            else
            {
                GameObject startButton = Instantiate(buttonPrefab, horizontalLayout2);
                string startText = selectedLanguage == language.ENGLISH ? "Start" : selectedLanguage == language.SPANISH ? "Empezar" : "";
                startButton.GetComponentInChildren<TextMeshProUGUI>().text = startText;
                RectTransform rectTransform = startButton.GetComponent<RectTransform>();
                //rectTransform.sizeDelta = new Vector2(320f, 90f); // Adjust the width and height as needed
                startButton.GetComponent<Button>().onClick.AddListener(() => StartButtonClick());
            }
        }
    }


    private void CreateUIElement(PropertyExtended prop, Transform parent)
    {
        // Create an input field, checkbox, or other UI element based on variable type
        if (prop.property.PropertyType == typeof(bool))
        {
            // Create a child GameObject for Toggle
            GameObject uiElement = Instantiate(yesNoQuestionPrefab, parent);
            uiElement.name = prop.property.Name;

            // Name label
            Text label = uiElement.transform.GetChild(0).GetComponent<Text>();
            label.text = prop.GetSentence();

            Button yesButton = uiElement.transform.GetChild(1).GetComponent<Button>();  // Button for Yes
            Button noButton = uiElement.transform.GetChild(2).GetComponent<Button>();   // Button for No

            string yesText = selectedLanguage == language.ENGLISH ? "Yes" : selectedLanguage == language.SPANISH ? "Sí" : "";
            yesButton.GetComponentInChildren<Text>().text = yesText;

            ChangeYesOrNoQuestionVisual(prop, yesButton, noButton, (bool)prop.property.GetValue(null));

            yesButton.onClick.AddListener(() =>
            {
                ChangeYesOrNoQuestionVisual(prop, yesButton, noButton, true);
            });

            noButton.onClick.AddListener(() =>
            {
                ChangeYesOrNoQuestionVisual(prop, yesButton, noButton, false);
            });

            // Info label
            Text label2 = uiElement.transform.GetChild(3).GetComponent<Text>();
            int numberOfLines = (int)(prop.GetSentence().Length / 109) + 1;
            if (string.IsNullOrEmpty(prop.GetInfo()))
            {
                RectTransform rect0 = uiElement.GetComponent<RectTransform>();
                rect0.sizeDelta = new Vector2(rect0.sizeDelta.x, 70 * numberOfLines);
                RectTransform rect1 = uiElement.transform.GetChild(0).GetComponent<RectTransform>();
                rect1.localPosition = new Vector3(rect1.localPosition.x, 0, rect1.localPosition.z);
                rect1.sizeDelta = new Vector2(rect1.sizeDelta.x, 70 * numberOfLines);
                RectTransform rect2 = uiElement.transform.GetChild(1).GetComponent<RectTransform>();
                rect2.localPosition = new Vector3(rect2.localPosition.x, 35 * (numberOfLines - 1), rect2.localPosition.z);
                RectTransform rect3 = uiElement.transform.GetChild(2).GetComponent<RectTransform>();
                rect3.localPosition = new Vector3(rect3.localPosition.x, 35 * (numberOfLines - 1), rect3.localPosition.z);

                Destroy(label2.gameObject);
            }
            else
            {
                label2.text = prop.GetInfo();
                RectTransform rectLabel = uiElement.transform.GetChild(3).GetComponent<RectTransform>();
                rectLabel.localPosition = new Vector3(rectLabel.localPosition.x, -5 - 35 * (numberOfLines - 1), rectLabel.localPosition.z);
            }
        }
        else if (prop.property.PropertyType == typeof(int) || prop.property.PropertyType == typeof(float))
        {
            // Create a child GameObject for InputField
            GameObject uiElement = Instantiate(inputFieldPrefab, parent);
            uiElement.name = prop.property.Name;

            // Name label
            Text label = uiElement.transform.GetChild(0).GetComponent<Text>();
            label.text = prop.GetSentence();

            TMP_InputField inputField = uiElement.GetComponentInChildren<TMP_InputField>();
            inputField.text = prop.property.GetValue(null).ToString();

            // Add event listener for input fields
            inputField.onValueChanged.AddListener((value) =>
            {
                SetValueFromInputField(prop.property, value);
            });

            // Info label
            Text label2 = uiElement.transform.GetChild(1).GetComponent<Text>();
            int numberOfLines = (int)(prop.GetSentence().Length / 109) + 1;
            if (string.IsNullOrEmpty(prop.GetInfo()))
            {
                RectTransform rect0 = uiElement.GetComponent<RectTransform>();
                rect0.sizeDelta = new Vector2(rect0.sizeDelta.x, 70 * numberOfLines);
                RectTransform rect1 = uiElement.transform.GetChild(0).GetComponent<RectTransform>();
                rect1.localPosition = new Vector3(rect1.localPosition.x, 0, rect1.localPosition.z);
                rect1.sizeDelta = new Vector2(rect1.sizeDelta.x, 70 * numberOfLines);
                RectTransform rect2 = uiElement.transform.GetChild(2).GetComponent<RectTransform>();
                rect2.localPosition = new Vector3(rect2.localPosition.x, 5 + 35 * (numberOfLines - 1), rect2.localPosition.z);

                Destroy(label2.gameObject);
            }
            else
            {
                label2.text = prop.GetInfo();
                RectTransform rectLabel = uiElement.transform.GetChild(1).GetComponent<RectTransform>();
                rectLabel.localPosition = new Vector3(rectLabel.localPosition.x, -5 - 35 * (numberOfLines - 1), rectLabel.localPosition.z);
            }
        }
        else if (prop.property.PropertyType == typeof(Vector2))
        {
            CreateVector2UIElements(prop, parent);
        }
        else if (prop.property.PropertyType == typeof(float[]))
        {
            CreateFloatArrayUIElements(prop, content);
        }
    }

    private void ChangeYesOrNoQuestionVisual(PropertyExtended prop, Button yesButton, Button noButton, bool status)
    {
        prop.property.SetValue(null, status);

        FontStyle yesStyle = status ? FontStyle.BoldAndItalic : FontStyle.Normal;
        FontStyle noStyle = !status ? FontStyle.BoldAndItalic : FontStyle.Normal;

        Color yesColor = status ? Color.yellow : Color.white;
        Color noColor = !status ? Color.yellow : Color.white;

        // Underline the text for Yes
        yesButton.GetComponentInChildren<Text>().fontStyle = yesStyle;
        yesButton.GetComponentInChildren<Text>().color = yesColor;
        yesButton.GetComponent<OnMouseOnEffect>().selected = status;
        if (prop.property.Name.Equals("sync"))
        {
            string buttonText = (selectedLanguage == language.SPANISH) ? "Síncrona" : (selectedLanguage == language.ENGLISH) ? "Synchronous":"";
            yesButton.GetComponentInChildren<Text>().text = buttonText;
        }
        // Remove underline from No
        noButton.GetComponentInChildren<Text>().fontStyle = noStyle;
        noButton.GetComponentInChildren<Text>().color = noColor;
        noButton.GetComponent<OnMouseOnEffect>().selected = !status;
        if (prop.property.Name.Equals("sync"))
        {
            string buttonText = (selectedLanguage == language.SPANISH) ? "Asíncrona" : (selectedLanguage == language.ENGLISH) ? "Asynchronous" : "";
            noButton.GetComponentInChildren<Text>().text = buttonText;
        }
    }

    private void CreateVector2UIElements(PropertyExtended prop, Transform parent)
    {
        Vector2 vector2Value = (Vector2)prop.property.GetValue(null);

        // Create a child GameObject for Vector2
        GameObject uiElement = Instantiate(inputFieldVector2Prefab, parent);
        uiElement.name = prop.property.Name;

        // Name label
        Text label = uiElement.transform.GetChild(0).GetComponent<Text>();
        label.text = prop.GetSentence();

        TMP_InputField inputFieldX = uiElement.transform.GetChild(2).GetComponent<TMP_InputField>();
        inputFieldX.text = vector2Value.x.ToString();

        // Add event listener for input fields
        inputFieldX.onValueChanged.AddListener((value) =>
        {
            // Update the Vector2 value when X changes
            // Note: You may want to add validation and handle parsing errors here
            Vector2 currentVector = (Vector2)prop.property.GetValue(null);
            currentVector.x = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
            prop.property.SetValue(null, currentVector);
        });

        TMP_InputField inputFieldY = uiElement.transform.GetChild(3).GetComponent<TMP_InputField>();
        inputFieldY.text = vector2Value.y.ToString();

        // Add event listener for input fields
        inputFieldY.onValueChanged.AddListener((value) =>
        {
            // Update the Vector2 value when Y changes
            // Note: You may want to add validation and handle parsing errors here
            Vector2 currentVector = (Vector2)prop.property.GetValue(null);
            currentVector.y = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
            prop.property.SetValue(null, currentVector);
        });

        // Info label
        Text label2 = uiElement.transform.GetChild(1).GetComponent<Text>();
        int numberOfLines = (int)(prop.GetSentence().Length / 109) + 1;
        if (string.IsNullOrEmpty(prop.GetInfo()))
        {
            RectTransform rect0 = uiElement.GetComponent<RectTransform>();
            rect0.sizeDelta = new Vector2(rect0.sizeDelta.x, 70 * numberOfLines);
            RectTransform rect1 = uiElement.transform.GetChild(0).GetComponent<RectTransform>();
            rect1.localPosition = new Vector3(rect1.localPosition.x, 0, rect1.localPosition.z);
            rect1.sizeDelta = new Vector2(rect1.sizeDelta.x, 70 * numberOfLines);
            RectTransform rect2 = uiElement.transform.GetChild(2).GetComponent<RectTransform>();
            rect2.localPosition = new Vector3(rect2.localPosition.x, 5 + 35 * (numberOfLines - 1), rect2.localPosition.z);
            RectTransform rect3 = uiElement.transform.GetChild(3).GetComponent<RectTransform>();
            rect3.localPosition = new Vector3(rect3.localPosition.x, 5 + 35 * (numberOfLines - 1), rect3.localPosition.z);

            Destroy(label2.gameObject);
        }
        else
        {
            label2.text = prop.GetInfo();
            RectTransform rectLabel = uiElement.transform.GetChild(1).GetComponent<RectTransform>();
            rectLabel.localPosition = new Vector3(rectLabel.localPosition.x, -5 - 35 * (numberOfLines - 1), rectLabel.localPosition.z);
        }
    }

    private void CreateFloatArrayUIElements(PropertyExtended prop, Transform parent)
    {
        // Create a child GameObject for Vector2
        GameObject uiElement = Instantiate(inputFieldFloat4Prefab, parent);
        uiElement.name = prop.property.Name;

        // Name label
        Text label = uiElement.transform.GetChild(0).GetComponent<Text>();
        label.text = prop.GetSentence();

        float[] floatArrayValue = (float[])prop.property.GetValue(null);
        if (floatArrayValue == null) { floatArrayValue = new float[4] { 35, 27, 32, 25 }; prop.property.SetValue(null, floatArrayValue); }

        for (int i = 0; i < floatArrayValue.Length; i++)
        {
            TMP_InputField arrayInputField = uiElement.transform.GetChild(i+2).GetComponent<TMP_InputField>();
            arrayInputField.text = floatArrayValue[i].ToString();

            int index = i; // Capture the index in the lambda expression
            // Add event listener for input fields
            arrayInputField.onValueChanged.AddListener((value) =>
            {
                // Update the float[] value when any element changes
                // Note: You may want to add validation and handle parsing errors here
                float[] currentArray = (float[])prop.property.GetValue(null);
                currentArray[index] = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                prop.property.SetValue(null, currentArray);
            });
        }

        // Info label
        Text label2 = uiElement.transform.GetChild(1).GetComponent<Text>();
        int numberOfLines = (int)(prop.GetSentence().Length / 109) + 1;
        if (string.IsNullOrEmpty(prop.GetInfo()))
        {
            RectTransform rect0 = uiElement.GetComponent<RectTransform>();
            rect0.sizeDelta = new Vector2(rect0.sizeDelta.x, 70 * numberOfLines);
            RectTransform rect1 = uiElement.transform.GetChild(0).GetComponent<RectTransform>();
            rect1.localPosition = new Vector3(rect1.localPosition.x, 0, rect1.localPosition.z);
            rect1.sizeDelta = new Vector2(rect1.sizeDelta.x, 70 * numberOfLines);
            RectTransform rect2 = uiElement.transform.GetChild(2).GetComponent<RectTransform>();
            rect2.localPosition = new Vector3(rect2.localPosition.x, 5 + 35 * (numberOfLines - 1), rect2.localPosition.z);
            RectTransform rect3 = uiElement.transform.GetChild(3).GetComponent<RectTransform>();
            rect3.localPosition = new Vector3(rect3.localPosition.x, 5 + 35 * (numberOfLines - 1), rect3.localPosition.z);
            RectTransform rect4 = uiElement.transform.GetChild(4).GetComponent<RectTransform>();
            rect4.localPosition = new Vector3(rect4.localPosition.x, 5 + 35 * (numberOfLines - 1), rect4.localPosition.z);
            RectTransform rect5 = uiElement.transform.GetChild(5).GetComponent<RectTransform>();
            rect5.localPosition = new Vector3(rect5.localPosition.x, 5 + 35 * (numberOfLines - 1), rect5.localPosition.z);

            Destroy(label2.gameObject);
        }
        else
        {
            label2.text = prop.GetInfo();
            RectTransform rectLabel = uiElement.transform.GetChild(1).GetComponent<RectTransform>();
            rectLabel.localPosition = new Vector3(rectLabel.localPosition.x, -5 - 35 * (numberOfLines - 1), rectLabel.localPosition.z);
        }
    }

    private void SetValueFromInputField(System.Reflection.PropertyInfo property, string value)
    {
        // Update the property value from the input field
        // Note: You may want to add validation and handle parsing errors here
        if (property.PropertyType == typeof(int))
        {
            property.SetValue(null, int.Parse(value));
        }
        else if (property.PropertyType == typeof(float))
        {
            property.SetValue(null, float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture));
        }
    }

    // JSON SAVE/LOAD
    private void SavePropertiesToJson()
    {
        // Register the Vector2Converter
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = { new Vector2Converter() }
        };

        // Create a dictionary to store property names and values
        Dictionary<string, object> propertiesDictionary = new Dictionary<string, object>();

        // Iterate through properties and store their values in the dictionary
        foreach (GroupOfProperties gProperty in GetCurrentCheckboxes().gProperties)
        {
            foreach (PropertyExtended prop in gProperty.properties)
            {
                propertiesDictionary[prop.property.Name] = prop.property.GetValue(null);
            }
        }

        // Convert the dictionary to a JSON string
        string jsonString = JsonConvert.SerializeObject(propertiesDictionary, Formatting.Indented);

        // Write the JSON string to the file
        File.WriteAllText(GetCurrentCheckboxes().filePath, jsonString);

        Debug.Log("Properties saved to JSON file: " + GetCurrentCheckboxes().filePath);
    }

    private void LoadPropertiesFromJson()
    {
        if (File.Exists(GetCurrentCheckboxes().filePath))
        {
            // Register the Vector2Converter
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = { new Vector2Converter() }
            };

            // Read the JSON string from the file
            string jsonString = File.ReadAllText(GetCurrentCheckboxes().filePath);

            // Deserialize the JSON string into a dictionary
            Dictionary<string, object> propertiesDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            // Apply the properties to the CheckboxManager
            foreach (var kvp in propertiesDictionary)
            {
                if (GetCurrentCheckboxes().Contains(kvp.Key))
                {
                    System.Reflection.PropertyInfo property = GlobalProperties.Instance.GetProperty((checkboxesId)GetCurrentCheckboxes().id, kvp.Key);

                    if (property != null)
                    {
                        // Convert the value to the correct type before setting it
                        object parsedValue = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(kvp.Value), property.PropertyType);
                        property.SetValue(null, parsedValue);
                    }
                }
            }

            Debug.Log("Properties loaded from JSON file: " + GetCurrentCheckboxes().filePath);
        }
        else
        {
            Debug.LogWarning("JSON file not found: " + GetCurrentCheckboxes().filePath);
        }
    }

    private void StartGame()
    {
        DataExtractorCSVVariables.Instance.GetSettingsAndSaveCsv();
        if (!Settings.play_level_1) { SceneManager.LoadScene("Level2"); }
        else { SceneManager.LoadScene("Level1"); }
    }

    private void StartButtonClick(bool welcomeScreen = false)
    {
        //We save before running start
        if (!welcomeScreen) { SavePropertiesToJson(); }

        //We load the rest of properties
        for (int i = 0; i <= (int)checkboxesId.LEVEL2; i++)
        {
            currGroup = i;

            // Condition to check if we want to load sync or async properties
            bool condition = Settings.sync && (currGroup == (int)checkboxesId.LEVEL1_ASYNC);
            condition |= !Settings.sync && (currGroup == (int)checkboxesId.LEVEL1_SYNC);
            condition |= !Settings.play_level_2 && (currGroup == (int)checkboxesId.LEVEL2);
            condition |= !Settings.play_level_1 && ((currGroup > (int)checkboxesId.SETTINGS) && (currGroup < (int)checkboxesId.LEVEL2));

            if (!condition) { LoadPropertiesFromJson(); }
        }

        //We start the game
        StartGame();
    }

    private void ContinueButtonClick()
    {
        SavePropertiesToJson();
        currGroup++; // We update the currGroup

        //Special conditions
        if (Settings.sync && (currGroup == (int)checkboxesId.LEVEL1_ASYNC)) { currGroup = (int)checkboxesId.LEVEL2; }
        else if (!Settings.sync && (currGroup == (int)checkboxesId.LEVEL1_SYNC)) { currGroup = (int)checkboxesId.LEVEL1_ASYNC; }

        if (!Settings.play_level_1 && (currGroup == (int)checkboxesId.LEVEL1)) { currGroup = (int)checkboxesId.LEVEL2; }
        else if (!Settings.play_level_2 && (currGroup == (int)checkboxesId.LEVEL2)) { currGroup++; }

        if (currGroup > (int)checkboxesId.LEVEL2) { StartGame(); }
        else { LoadAndGenerateUI(); }
    }

    private void AdvancedSettingsButtonClick()
    {
        SavePropertiesToJson();
        advancedStatus = GlobalGroupsOfCheckboxes.ADVANCED;  // We update the advanced status

        LoadAndGenerateUI();    // We load the UI
    }

    private void BackButtonClick()
    {
        SavePropertiesToJson();

        if (advancedStatus) { advancedStatus = GlobalGroupsOfCheckboxes.GENERAL; } // We update the advanced status
        else
        {
            currGroup--;    // We update the currGroup
            //Special conditions
            bool skipLevel1Cond = (currGroup == (int)checkboxesId.LEVEL1) || (currGroup == (int)checkboxesId.LEVEL1_SYNC) || (currGroup == (int)checkboxesId.LEVEL1_ASYNC);
            if (!Settings.play_level_1 && skipLevel1Cond) { currGroup = (int)checkboxesId.SETTINGS; }

            if (Settings.sync && (currGroup == (int)checkboxesId.LEVEL1_ASYNC)) { currGroup = (int)checkboxesId.LEVEL1_SYNC; }
            else if (!Settings.sync && (currGroup == (int)checkboxesId.LEVEL1_SYNC)) { currGroup = (int)checkboxesId.LEVEL1; }
        }

        if (currGroup < (int)checkboxesId.SETTINGS) { GenerateUIWelcomeScreen(); currGroup = 0; }
        else { LoadAndGenerateUI(); }
    }
}

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        serializer.Serialize(writer, value.x);
        writer.WritePropertyName("y");
        serializer.Serialize(writer, value.y);
        writer.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader reader, System.Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject)
        {
            // Move to the first property
            reader.Read();

            float x = 0f;
            float y = 0f;

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = (string)reader.Value;

                // Move to the property value
                reader.Read();

                // Check the property name and deserialize the value accordingly
                switch (propertyName)
                {
                    case "x":
                        x = serializer.Deserialize<float>(reader);
                        break;
                    case "y":
                        y = serializer.Deserialize<float>(reader);
                        break;
                    default:
                        // Skip unknown properties
                        reader.Skip();
                        break;
                }

                // Move to the next property or the end of the object
                reader.Read();
            }

            // Ensure the object ends correctly
            if (reader.TokenType != JsonToken.EndObject)
            {
                throw new JsonSerializationException("Unexpected token type while deserializing Vector2.");
            }

            return new Vector2(x, y);
        }

        throw new JsonSerializationException("Unexpected token type while deserializing Vector2.");
    }
}