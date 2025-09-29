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
  const { scene, materials } = useGLTF(`/complete${props.config.headId}${props.config.bodyId}${props.config.tailId}.glb`)

  useEffect(() => {
    Object.keys(materials).forEach((matName : string) => {
      if (matName != "Material.003" && matName != "Material.004") {
        (materials[matName] as any).color.set(props.config.color);
      }
    });
  }, [materials, props])

  const fishRef = useRef<Group>(null!)
  const startY = useRef<number | null>(null)
  const [swimming, setSwimming] = useState(false)
  const startTime = useRef<number | null>(null)

  useFrame((state) => {
    if (!swimming || !props.onSwimAway) return
  
    if (startTime.current === null) startTime.current = state.clock.elapsedTime
    const elapsed = state.clock.elapsedTime - startTime.current
  
    // Speed controls
    const speed = 5 // units/sec
    const arcRadius = 2.5
    const arcLength = Math.PI * arcRadius // half circle length
    const arcDuration = arcLength / speed // time to complete arc
  
    if (elapsed < arcDuration) {
      // --- Arc path ---
      const t = elapsed / arcDuration // 0 → 1
      const angle = Math.PI * t
      const x = Math.cos(angle) * arcRadius
      const z = Math.sin(angle) * arcRadius
  
      fishRef.current!.position.set(x, 0, z)
  
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

useGLTF.preload('/complete1.glb')
