import { useThree } from "@react-three/fiber";
import { useEffect } from "react";
import * as THREE from "three";

export function OverheadCamera() {
  const { camera } = useThree();

  useEffect(() => {
    // Point camera straight down toward origin
    camera.up.set(0, 0, -1);         // set "up" direction so it's consistent
    camera.lookAt(new THREE.Vector3(0, 0, 0));
  }, [camera]);

  return null;
}