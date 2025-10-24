import { Center, useGLTF } from '@react-three/drei'
import { useEffect, useState, useRef } from 'react';
import { useFrame } from '@react-three/fiber';
import type { FishConfig } from '@/utils/types';
import type { Group } from 'three';

interface FishModelProps {
  config: FishConfig,
  onSwimStart?: () => void;
  onSwimAway?: () => void;
}

export function CompleteFishModel(props: FishModelProps) {
  const { scene, materials } = useGLTF(`/fish${props.config.headId}${props.config.bodyId}${props.config.tailId}.glb`)
  // If you want to find the armature explicitly:
  const armature = scene.getObjectByName('Armature')
  const tailBone = armature?.getObjectByName('tail'); 
  // const bodyBone = armature?.getObjectByName('body'); 
  const audioRef = useRef(new Audio('/water-splash-199583.mp3'));


  useEffect(() => {
    Object.keys(materials).forEach((matName : string) => {
      if (matName.slice(0, 3) !== "Mat") { // hacky way to avoid coloring eyes
        (materials[matName] as any).color.set(props.config.color);
      }
    });
  }, [materials, props])

  const fishRef = useRef<Group>(null!)
  const startY = useRef<number | null>(null)
  const [swimming, setSwimming] = useState(false)
  const [swamAway, setSwamAway] = useState(false);
  const startTime = useRef<number | null>(null)

  useFrame((state) => {
    const t = state.clock.elapsedTime
    // wiggle speed and amplitude
    const frequency = 4   // how fast the fish wiggles
    const tailAmp   = 0.2 // radians (~17°)
    // const bodyAmp = 0.05;

    // bodyBone.rotation.z = Math.sin(t * frequency) * bodyAmp

    if (tailBone) {
      tailBone.rotation.z = Math.sin(t * frequency + Math.PI / 4) * tailAmp
    } 

    if (!swimming || !props.onSwimAway) {
      const wiggle = Math.sin(t * frequency) * 0.05 // 3° sway
      fishRef.current.rotation.y = wiggle
      return;
    }
  
    if (startTime.current === null) startTime.current = state.clock.elapsedTime
    const elapsed = state.clock.elapsedTime - startTime.current
  
    // Speed controls
    const speed = 8 // units/sec
    const arcRadius = 2.5
    const arcLength = Math.PI * arcRadius // half circle length
    const arcDuration = arcLength / speed // time to complete arc

    const lateralSway = Math.sin(t * frequency) * 0.05 // side-to-side wiggle
    fishRef.current.position.x += lateralSway
  
    if (elapsed < arcDuration) {
      // --- Arc path ---
      const t = elapsed / arcDuration // 0 → 1
      const angle = Math.PI * t
      const x = Math.cos(angle) * arcRadius
      const z = Math.sin(angle) * arcRadius
  
      fishRef.current!.position.set(x, 0, z)
      fishRef.current.rotation.z = Math.sin(angle) * 0.2 // tilt into the curve
  
      // Face along tangent
      const dx = -Math.sin(angle)
      const dz = Math.cos(angle)
      const heading = Math.atan2(dx, dz)
      fishRef.current!.rotation.y = heading
    } else {
      // --- Straight swim after arc ---
      const extraTime = elapsed - arcDuration
      const distance = speed * extraTime
  
      // End of arc coordinates
      const endX = Math.cos(Math.PI) * arcRadius // -radius
      const endZ = Math.sin(Math.PI) * arcRadius // 0
  
      // Move forward along Z axis
      fishRef.current!.position.set(endX, 0, endZ - distance)
      fishRef.current!.rotation.y = Math.PI // facing +Z
  
      // Once fish is far enough, trigger exit
      if (endZ - distance < 100) {
        props.onSwimAway()
      }

      // Once fish is off screen trigger splash sound, but only once
      if (fishRef.current!.position.z < -20 && !swamAway) {
        setSwamAway(true);
        audioRef.current.currentTime = 0;
        audioRef.current.play();
      }
    }
  })

  return (
    <group 
      ref={fishRef} 
      onPointerDown={(e) => {
        startY.current = e.clientY // record where swipe started
      }}
      onPointerUp={() => {
        if (startY.current !== null && props.onSwimStart) {
          // const deltaY = e.clientY - startY.current
          // if (deltaY < -2) { // user dragged upward
            setSwimming(true);
            props.onSwimStart();
          // }
        }
      }}
      >
      <Center>
        <primitive object={scene} />
      </Center>
    </group>
  )
}
