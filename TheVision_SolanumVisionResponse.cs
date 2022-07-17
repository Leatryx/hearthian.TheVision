﻿using UnityEngine;
using TheVision.Utilities.ModAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace TheVision.CustomProps
{
    class TheVision_SolanumVisionResponse : MonoBehaviour



    {
        public NomaiConversationManager _nomaiConversationManager;
        public SolanumAnimController _solanumAnimController;
        public NomaiWallText solanumVisionResponse;
        public OWAudioSource PlayerHeadsetAudioSource;

        private static readonly int MAX_WAIT_FRAMES = 20;
        private int waitFrames = 0;
        private bool visionEnded = false;
        private bool doneHijacking = false;
        private bool hasStartedWriting = false;


        void Update()
        {
            

            if (!visionEnded) return;
            if (doneHijacking) return;
            if (waitFrames > 0) { waitFrames--; return; }

            if (!hasStartedWriting)
            {
                NomaiWallText responseText = GameObject.Find("QuantumMoon_Body/Sector_QuantumMoon/State_EYE/NomaiWallText").GetComponent<NomaiWallText>();
                
                // one-time code that runs after waitFrames are up
                _solanumAnimController.OnWriteResponse += (int unused) => responseText.Show();
                _solanumAnimController.StartWritingMessage();
                hasStartedWriting = true;

            }


            if (!_solanumAnimController.isStartingWrite && !solanumVisionResponse.IsAnimationPlaying())
            {
                // drawing custom text                
               // var customResponse = GameObject.Find("QuantumMoon_Body/Sector_QuantumMoon/State_Eye/NomaiWallText");
               // customResponse.GetAddComponent<NomaiWallText>().Show();

                _solanumAnimController.StopWritingMessage(gestureToText: false);
                _solanumAnimController.StopWatchingPlayer();
                doneHijacking = true;

                

                // Spawning SolanumCopies and Signals on vision response
                TheVision.Instance.ModHelper.Events.Unity.FireInNUpdates(
          () => TheVision.Instance.SpawnSolanumCopy(TheVision.Instance.ModHelper.Interaction.GetModApi<INewHorizons>("xen.NewHorizons")), 10);                
                TheVision.Instance.SpawnSignals();
                // TheVision.Instance.DisabledPropsOnStart(true);                
            }

        }

        public void OnVisionEnd()

        {

            // SFX on QM after Solanumptojection
            PlayerHeadsetAudioSource = GameObject.Find("Player_Body").AddComponent<OWAudioSource>();
            PlayerHeadsetAudioSource.enabled = true;
            PlayerHeadsetAudioSource.AssignAudioLibraryClip((AudioType)2252); // shattering sound 2428 //2697 - station flicker
            PlayerHeadsetAudioSource.SetMaxVolume(maxVolume: 0.3f);
            PlayerHeadsetAudioSource.Play();


           // var lightning = GameObject.Find("QuantumMoon_Body/Sector_QuantumMoon/LightningGenerator_GD_CloudLightningInstance(Clone)").GetComponent<CloudLightning>();
          // lightning._lightTimer = 5f;
            



            TheVision.Instance.ModHelper.Console.WriteLine("PROJECTION COMPLETE");
            _nomaiConversationManager.enabled = false;
            visionEnded = true;
            waitFrames = MAX_WAIT_FRAMES;

            var effect = GameObject.Find("Player_Body/PlayerCamera/ScreenEffects/LightFlickerEffectBubble").GetComponent<LightFlickerController>();
            effect.FlickerOffAndOn(offDuration: 6.5f, onDuration: 0.5f);
            
            // QuantumMoonLightningGenerator generator


        }

    }




}
    // hijacking Solanum's conversation controller:

    //         // under NomaiConversationManager
    // _activeResponseText.Show();
    // nomaiConversationManager.enabled = false;
    //_solanumAnimController.StartWritingMessage();
    //         // then every frame,
    //if (!_solanumAnimController.isStartingWrite && !_activeResponseText.IsAnimationPlaying())
    //{
    //	_solanumAnimController.StopWritingMessage(gestureToText: true);
    //  _solanumAnimController.StopWatchingPlayer();
    //}

