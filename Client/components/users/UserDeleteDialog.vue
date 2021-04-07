<template>
  <div></div>
</template>

<script lang="ts">
import { Component, Vue, Prop} from "nuxt-property-decorator"

@Component({})
export default class UserDeleteDialog extends Vue {
  @Prop()
  active!: boolean
}
</script>
