import { Flex, Box } from "@chakra-ui/react"
import MoveButton from "@/components/MoveButton"
import ShootButton from "@/components/ShootButton"

export default function ActionButtons() {
  const direction = [0, 1, 2, 3]

  return (
    <Flex flexDirection="column" alignItems="center">
      <Box mb="2">
        <MoveButton direction={direction[0]} />
      </Box>
      <Flex>
        <Box mr="2">
          <MoveButton direction={direction[3]} />
        </Box>
        <ShootButton />
        <Box ml="2">
          <MoveButton direction={direction[1]} />
        </Box>
      </Flex>
      <Box mt="2">
        <MoveButton direction={direction[2]} />
      </Box>
    </Flex>
  )
}
