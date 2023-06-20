import { VStack, HStack } from "@chakra-ui/react"
import InitializeShipButton from "@/components/InitializeShipButton"
import UpgradeShipButton from "@/components/UpgradeShipButton"
import SpawnShipButton from "@/components/SpawnShipButton"

export default function ShipSetupButtons() {
  return (
    <VStack>
      <InitializeShipButton />
      <HStack>
        <UpgradeShipButton />
        <SpawnShipButton />
      </HStack>
    </VStack>
  )
}
