<template>
  <v-col>
    <template v-if="fetched">
      <template v-if="!isPersonal">
        <v-toolbar dense flat>
          <v-btn text @click.stop="onUserClosed()"
            ><v-icon>mdi-keyboard-return</v-icon>Назад</v-btn
          >
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-divider></v-divider>
      </template>
      <div class="py-6">
        <div class="d-flex justify-space-between">
          <span>Имя:</span><span>{{ userData.firstName }}</span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Фамилия:</span><span>{{ userData.lastName }}</span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Эл.адрес:</span><span>{{ userData.email }}</span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Роль:</span><span>{{ userData.role }}</span>
        </div>
      </div>
      <v-divider></v-divider>
      <v-btn class="mt-3 mr-2">Изменить данные</v-btn>
      <template v-if="!isPersonal">
        <v-btn class="mt-3 mr-2">Изменить роль</v-btn>
        <v-btn class="mt-3 mr-2">Изменить пароль</v-btn>
        <v-btn class="mt-3 mr-2">Удалить</v-btn>
      </template>
    </template>
  </v-col>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import { identityStore, usersStore } from "~/store";
import { UserGetDTO } from "~/types/Identity/UserDTO";

@Component({})
export default class UserData extends Vue {
  @Prop()
  user?: UserGetDTO | null;

  fetched = false;

  get isPersonal() {
    return !this.user;
  }

  get userData() {
    if (!this.isPersonal) {
      return this.user;
    }
    return identityStore.userData;
  }

  onUserClosed() {
    usersStore.SELECTED_USER_CLEARED();
  }

  mounted() {
    if (this.isPersonal) {
      identityStore.fetchData().then((_) => (this.fetched = true));
    } else {
      this.fetched = true;
    }
  }
}
</script>

