<template>
  <v-btn text outlined><slot></slot></v-btn>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";

@Component({})
export default class CustomButton extends Vue {
  @Prop({ default: () => "none" })
  type!: "none" | "add" | "edit" | "delete" | "history" | "check" | "uncheck";

  @Prop({ default: () => "" })
  text!: string;

  @Prop({ default: () => false })
  showIcon!: boolean;

  @Prop()
  icon!: string | undefined;

  @Prop()
  to!: string | undefined;
}
</script>

