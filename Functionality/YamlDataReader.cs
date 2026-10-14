using SadConsole.Components;
using SadConsole.Entities;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Wizard5;
using YamlDotNet.RepresentationModel;




public class PostConstructEventHolder
{
    public List<GameEvent> postConstructEvents;

    public PostConstructEventHolder(List<GameEvent> postevents)
    {
        this.postConstructEvents = postevents;
    }

}

public static class YamlDataReader
{

    public static Dictionary<string, GameEntity> ConstructedEntities = new Dictionary<string, GameEntity>();


    public static GameEntity CreateEntityCached(string dir, string entname)
    {
        if (ConstructedEntities.ContainsKey(entname))
        {
            return ConstructedEntities[entname].Copy(new GameEntity());
        }

        var ent = GetEntityFromData(dir, entname);
        
        ConstructedEntities.Add(entname, ent.Copy(new GameEntity()));
        return ent;
    }


    public static string GameDataPath
    {
        get
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\");
        }
    }

    public static string YamlDataExtention = ".yaml";



    public static YamlMappingNode GetYamlRootNode(string path)
    {
        YamlMappingNode rootnode = null;
        if (!File.Exists(path))
        {
            //Debug.Log("No file exists at path " + path);
        }
        using (var reader = new StreamReader(path))
        {
            // Load the stream
            var yaml = new YamlStream();
            yaml.Load(reader);
            rootnode = (YamlMappingNode)yaml.Documents[0].RootNode;
        }
        return rootnode;
    }

    //public static ColorList GetColorListFromData()
    //{
    //    var colorlistRootNode = GetYamlRootNode(GameDataPath + "ColorList" + YamlDataExtention);
    //    return DecodeYamlObject(typeof(ColorList), colorlistRootNode, new List<object>()) as ColorList;
    //}

    public static GameEntity GetEntityFromData(string dir, string name, string eventName = null)
    {
        GameEntity entity = null;
        YamlMappingNode entRootNode = null;
        if (string.IsNullOrEmpty(dir))
        {
            entRootNode = GetYamlRootNode(GameDataPath + name + YamlDataExtention);
        }
        else
        {
            entRootNode = GetYamlRootNode(GameDataPath + dir + "\\" + name + YamlDataExtention);
        }
        entity = DecodeYamlObject(typeof(GameEntity), entRootNode, new List<object>()) as GameEntity;
        foreach (var comp in entity.Components)
        {
            comp.Owner = entity;
        }
        if (!string.IsNullOrEmpty(eventName))
        {
            var eventRootNode = GetYamlRootNode(GameDataPath + "Events" + "\\" + eventName + YamlDataExtention);
            var playerEvents = DecodeYamlObject(typeof(PostConstructEventHolder), eventRootNode, new List<object>()) as PostConstructEventHolder;
            foreach (var gameevent in playerEvents.postConstructEvents)
            {
                entity.ResolveGameEvent(gameevent);
            }

        }

        return entity;
    }


    private static object DecodeYamlObject(Type stringtype, YamlMappingNode component, List<object> constructParams)
    {

        System.Reflection.ConstructorInfo ctor = null;
        bool hasParamConstructor = false;

        if (stringtype.Name == "IGameComponent")
        {
            foreach (var item in component.Children)
            {
                if (item.Key.ToString() == "ComponentName")
                {

                    string targetName = item.Value.ToString().Contains(".") ? item.Value.ToString().Split('.').Last() : item.Value.ToString();
                    stringtype = typeof(GameEntity).Assembly
                        .GetTypes()
                        .FirstOrDefault(t => t.Name == targetName);

                    constructParams.Add(new GameEntity());
                    break;
                }
            }
        }

        if (stringtype.Name == "GameEvent")
        {
            foreach (var item in component.Children)
            {
                if (item.Key.ToString() == "EventName")
                {
                    stringtype = typeof(GameEvent).Assembly.GetType(item.Value.ToString());
                    break;
                }
            }
        }


        if (stringtype.Name == "Color")
        {
            return DecodeColor(component);
        }
        

        foreach (var possiblector in stringtype.GetConstructors())
        {
            if (possiblector.GetParameters().Length > 0)
            {
                hasParamConstructor = true;
                ctor = possiblector;
                break;
            }
        }

        if (!hasParamConstructor)
        {
            return Activator.CreateInstance(stringtype);

        }

        var p = ctor.GetParameters();
        List<string> LostParams = new List<string>();
        foreach (var child in component.Children)
        {

            if (StrCmp(child.Key.ToString(), "ComponentName") || StrCmp(child.Key.ToString(), "EventName"))
            {
                continue;
            }
            bool foundparam = false;
            foreach (var param in p)
            {

                if (StrCmp(param.Name, child.Key.ToString()))
                {
                    var paramType = param.ParameterType;

                    if (paramType.IsPrimitive || paramType.IsEnum || paramType.Name == "String")
                    {
                        constructParams.Add(DecodePrimativeType(child, paramType));
                        foundparam = true;

                    }
                    else
                    {
                        if (child.Value.ToString()[0] == '>')
                        {
                            //the value is in a different file
                            var filename = child.Value.ToString().Substring(1);
                            //var foundRootNode = GetRootNode(GameDataPath + newFile + DataExtention);
                            //var referencedObject = DecodeObject(typeof(GameEntity), foundRootNode, new List<object>()) as GameEntity;
                            constructParams.Add(GetEntityFromData("Items", filename));
                            foundparam = true;


                        }

                        //is it a mapping node (class)
                        var mapping = child.Value as YamlMappingNode;
                        if (mapping != null)
                        {
                            var interorinst = DecodeYamlObject(paramType, mapping, new List<object>());
                            constructParams.Add(interorinst);
                            foundparam = true;

                        }

                        //is it a sequence node (list)
                        var sequence = child.Value as YamlSequenceNode;
                        if (sequence != null)
                        {
                            var list = DecodeList(paramType, sequence);
                            constructParams.Add(list);
                            foundparam = true;

                        }

                    }

                    if (!foundparam)
                    {
                        LostParams.Add(param.Name);
                    }

                    break;
                }
            }
            if (!foundparam)
            {
                foreach (var lastcheck in component.Children)
                {

                    //try going deeper into stupid ymal file
                    var mapping = lastcheck.Value as YamlMappingNode;
                    if (mapping != null)
                    {
                        return DecodeYamlObject(stringtype, mapping, new List<object>());
                    }

                }
                string paramstring = "";
                foreach (var param in p)
                {
                    paramstring += param.Name + " " + " " + param.ParameterType;
                }

                //Debug.Log("No constructor param found for " + child.Key.ToString() + " was expecting param ");
            }
        }

        object? inst;
        try
        {
            inst = Activator.CreateInstance(stringtype, args: constructParams.ToArray());
        }
        catch (System.Reflection.TargetInvocationException ex)
        {
            var realException = ex.InnerException;
            System.Diagnostics.Debug.WriteLine($"Real Error: {realException.Message}");
            System.Diagnostics.Debug.WriteLine($"Real Stack Trace: {realException.StackTrace}");
            throw realException;
        }

        return inst;
    }

    private static IList DecodeList(Type paramType, YamlSequenceNode sequence)
    {
        if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type listType = paramType.GetGenericArguments()[0];
            IList NewList = CreateList(listType);

            foreach (var node in sequence.Children)
            {
                if (listType.IsPrimitive || listType.IsEnum)
                {
                    NewList.Add(GetPrimativeType(node, listType));
                }
                else
                {
                    var interorMapping = node as YamlMappingNode;
                    if (interorMapping != null)
                    {
                        NewList.Add(DecodeYamlObject(listType, interorMapping, new List<object>()));
                    }
                    //todo - lists within lists - fuck you
                }
            }

            return NewList;

        }
        else
        {
            //Debug.Log("Somthing bad happened", LogCategory.Error);
            return null;
        }
    }

    public static IList CreateList(Type myType)
    {
        Type genericListType = typeof(List<>).MakeGenericType(myType);
        return (IList)Activator.CreateInstance(genericListType);
    }

    public static bool StrCmp(string A, string B)
    {
        return A.Equals(B, StringComparison.OrdinalIgnoreCase);
    }



    private static SadRogue.Primitives.Color DecodeColor(YamlMappingNode component)
    {
        float r=0.0f;
        float g=0.0f;
        float b=0.0f;
        float a=0.0f;

        if (component.Children.Count > 1)
        {
            foreach (var item in component.Children)
            {
                if (item.Key.ToString() == "R")
                {
                    r = byte.Parse(item.Value.ToString());
                }
                if (item.Key.ToString() == "G")
                {
                    g = byte.Parse(item.Value.ToString());
                }
                if (item.Key.ToString() == "B")
                {
                    b = byte.Parse(item.Value.ToString());
                }
                if (item.Key.ToString() == "A")
                {
                    a = byte.Parse(item.Value.ToString());
                }
            }
        }
        else
        {
            foreach (var item in component.Children)
            {
                if (item.Key.ToString() == "NamedColor")
                {
                    if (GameSettings.GameColors.ContainsKey(item.Value.ToString()))
                    {
                        return GameSettings.GameColors[item.Value.ToString()];
                    }

                }
            }
        }

        return new SadRogue.Primitives.Color(r,g,b,a);
    }

    private static object DecodePrimativeType(KeyValuePair<YamlNode, YamlNode> child, Type paramType)
    {
        var foo = TypeDescriptor.GetConverter(paramType);
        var paraminst = (foo.ConvertFromInvariantString(child.Value.ToString()));
        return paraminst;
    }
    private static object GetPrimativeType(YamlNode child, Type paramType)
    {
        var foo = TypeDescriptor.GetConverter(paramType);
        var paraminst = (foo.ConvertFromInvariantString(child.ToString()));
        return paraminst;
    }

}
