<template>
  <v-col>
    <template v-if="fetched">
      <v-toolbar flat>
        <v-spacer></v-spacer>
        <v-btn text @click.stop="openCreateDialog()">Добавить пользователя</v-btn>
        <v-spacer></v-spacer>
      </v-toolbar>
      <v-divider></v-divider>
      <v-list rounded dense class="mt-3 pa-0">
        <v-list-item
          v-for="user in users"
          :key="user.id"
          @click="onUserSellected(user)"
          dense
        >
          <v-list-item-content class="ma-0 pa-0">
            <div class="d-flex justify-space-between py-2">
              <span class="text-body-1">{{ getFullname(user) | textTruncate(30) }}</span>
              <v-chip small>
                <template>{{ user.roleLocalized }}</template>
              </v-chip>
            </div>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </template>
    <UserCreateDialog v-model="createDialog" />
  </v-col>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { identityStore, usersStore } from "~/store";
import { UserGetDTO } from "~/models/Identity/UserDTO";
import UserCreateDialog from "~/components/users/UserCreateDialog.vue";

@Component({
  components: {
    UserCreateDialog
  }
})
export default class UsersList extends Vue {
  fetched = false;
  createDialog = false;

  get users() {
    return usersStore.users.filter(user => user.id != identityStore.userData?.id);
  }

  openCreateDialog(){
    this.createDialog = true;
  }

  onUserSellected(user: UserGetDTO) {
    usersStore.getUser(user.id)
  }

  getFullname(user: UserGetDTO) {
    return `${user.firstName} ${user.lastName}`;
  }

  mounted() {
    usersStore.getUsers().then((_) => {
      this.fetched = true;
    });
  }
}
</script>

