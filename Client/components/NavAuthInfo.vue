<template>
  <v-btn v-if="isAuthenticated" rounded text large @click="onClick">
    <span class="text-h6 font-weight-medium mr-2">
      {{ fullName }}
    </span>
    <v-chip v-if="isAdministrator" small class="primary text-caption">Администратор</v-chip>
    <v-chip v-if="isRoot" small class="warning text-caption">Супер Администратор</v-chip>
    <v-chip v-if="!isAdministrator && !isRoot" small class="text-caption">Пользователь</v-chip>
  </v-btn>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { identityStore } from "~/store";

@Component({})
export default class NavAuth extends Vue {
  get isAuthenticated() {
    return identityStore.isAuthenticated;
  }

  get fullName() {
    return identityStore.fullName;
  }

  get isAdministrator() {
    return identityStore.isAdministrator;
  }

  get isRoot() {
    return identityStore.isRoot;
  }

  onClick(){
    this.$router.push("/users")
  }
}
</script>
