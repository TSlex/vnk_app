<template>
  <v-row justify="center" class="text-center" v-if="loaded">
    <v-col cols="4" class="mt-4">
      <v-sheet rounded="lg" class="py-2">
        <div class="px-3 mb-2">
          <div class="d-flex justify-space-between mb-2">
            <span>Название:</span><span></span>
          </div>
        </div>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({})
export default class extends Vue {
  loaded = false;
}
</script>
