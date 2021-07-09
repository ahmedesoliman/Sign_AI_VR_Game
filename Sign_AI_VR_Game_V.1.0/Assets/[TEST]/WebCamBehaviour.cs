using System;
using UnityEngine;
using System.Collections.Generic;
 
public class WebCamBehaviour : MonoBehaviour
{
    /// <summary>
    /// Meta reference to the camera
    /// </summary>
    public Material CameraMaterial = null;
   
    /// <summary>
    /// The number of frames per second
    /// </summary>
    private int m_framesPerSecond = 0;
   
    /// <summary>
    /// The current frame count
    /// </summary>
    private int m_frameCount = 0;
   
    /// <summary>
    /// The frames timer
    /// </summary>
    private DateTime m_timerFrames = DateTime.MinValue;
   
    /// <summary>
    /// The selected device index
    /// </summary>
    private int m_indexDevice = -1;
   
    /// <summary>
    /// The web cam texture
    /// </summary>
    private WebCamTexture m_texture = null;
   
    // Use this for initialization
    void Start()
    {
        if (null == CameraMaterial)
        {
            throw new ApplicationException("Missing camera material reference");
        }
       
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
    }
   
    void OnGUI()
    {
        if (m_timerFrames < DateTime.Now)
        {
            m_framesPerSecond = m_frameCount;
            m_frameCount = 0;
            m_timerFrames = DateTime.Now + TimeSpan.FromSeconds(1);
        }
        ++m_frameCount;
       
        GUILayout.Label(string.Format("Frames per second: {0}", m_framesPerSecond));
       
        if (m_indexDevice >= 0 && WebCamTexture.devices.Length > 0)
        {
            GUILayout.Label(string.Format("Selected Device: {0}", WebCamTexture.devices[m_indexDevice].name));
        }
       
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            GUILayout.Label("Has WebCam Authorization");
            if (null == WebCamTexture.devices)
            {
                GUILayout.Label("Null web cam devices");
            }
            else
            {
                GUILayout.Label(string.Format("{0} web cam devices", WebCamTexture.devices.Length));
                for (int index = 0; index < WebCamTexture.devices.Length; ++index)
                {
                    var device = WebCamTexture.devices[index];
                    if (string.IsNullOrEmpty(device.name))
                    {
                        GUILayout.Label("unnamed web cam device");
                        continue;
                    }
                   
                    if (GUILayout.Button(string.Format("web cam device {0}{1}{2}",
                                                       m_indexDevice == index
                                                       ? "["
                                                       : string.Empty,
                                                       device.name,
                                                       m_indexDevice == index ? "]" : string.Empty),
                                         GUILayout.MinWidth(200),
                                         GUILayout.MinHeight(50)))
                    {
                        m_indexDevice = index;
                       
                        // stop playing
                        if (null != m_texture)
                        {
                            if (m_texture.isPlaying)
                            {
                                m_texture.Stop();
                            }
                        }
                       
                        // destroy the old texture
                        if (null != m_texture)
                        {
                            UnityEngine.Object.DestroyImmediate(m_texture, true);
                        }
                       
                        // use the device name
                        m_texture = new WebCamTexture(device.name);
                       
                        // start playing
                        m_texture.Play();
                       
                        // assign the texture
                        CameraMaterial.mainTexture = m_texture;
                    }
                }
            }
        }
        else
        {
            GUILayout.Label("Pending WebCam Authorization...");
        }
    }
   
    // Update is called once per frame
    private void Update()
    {
        if (null != m_texture &&
            m_texture.didUpdateThisFrame)
        {
            CameraMaterial.mainTexture = m_texture;
        }
    }
}
