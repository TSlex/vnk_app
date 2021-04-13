<template>
  <v-container class="text-center" fluid>
    <br />
    <br />
    <br />
    <br />
    <h1>
      <v-icon class="text-h1 red--text accent-2">mdi-access-point-off</v-icon>
    </h1>
    <h1 class="text-h3">Сервер оффлайн!</h1>
    <h3 class="text-h6 mt-4">
      Проверьте сервер на работоспособность. <br />
      Если ошибка не исчезла, свяжитесь с разработчиком
    </h3>
    <div class="mt-4">
      <a href="mailto:alex10119996@gmail.com" class="text-body-1"
        >alex10119996@gmail.com</a
      >
    </div>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { commonStore } from "~/store";

@Component({})
export default class MyClass extends Vue {
  mounted() {
    commonStore.checkServer().then((online) => {
      if (online) {
        this.$router.push("/");
      }
    });
  }


}
</script>
