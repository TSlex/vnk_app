<template>
  <v-col>
    <template v-if="fetched">
      <v-toolbar flat>
        <v-spacer></v-spacer>
        <v-btn text>Добавить пользователя</v-btn>
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
              <span class="text-body-1">{{ getFullname(user) }}</span>
              <v-chip small>
                <template>{{ user.role }}</template>
              </v-chip>
            </div>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </template>
  </v-col>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { usersStore } from "~/store";
import { UserGetDTO } from "~/types/Identity/UserDTO";

@Component({})
export default class UsersList extends Vue {
  fetched = false;

  get users() {
    return usersStore.users;
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

