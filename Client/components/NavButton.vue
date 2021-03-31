<template>
  <!-- <v-btn depressed tile large :to="to" active-class="v-btn--hide-active"><slot></slot></v-btn> -->
  <v-btn depressed tile large :to="to"><slot></slot></v-btn>
</template>

<script>
export default {
  props: ["to"],
};
</script>

