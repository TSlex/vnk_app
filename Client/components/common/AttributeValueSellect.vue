<template>
  <div v-if="fetched">
    <CustomValueField
      :dataType="attributeType.dataType"
      v-model="customValue"
      :label="`Значение`"
      v-if="!attributeType.usesDefinedValues"
    />
    <v-select
      v-else
      v-model="valueId"
      :items="attributeType.values"
      item-text="value"
      item-value="id"
      label="Значение"
      single-line
    ></v-select>
    <v-select
      v-if="attributeType.usesDefinedUnits"
      v-model="unitId"
      :items="attributeType.units"
      item-text="value"
      item-value="id"
      label="Ед. измерения"
      single-line
    ></v-select>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import CustomValueField from "~/components/common/CustomValueField.vue";
import { AttributeTypeDetailsGetDTO } from "~/models/AttributeTypeDTO";

@Component({
  components: {
    CustomValueField,
  },
})
export default class AttributeValueSellect extends Vue {
  @Prop()
  value!: {
    customValue: string;
    valueId: null | number;
    unitId: null | number;
  };

  @Prop({ default: null })
  typeId!: number | null;

  fetched = false;

  attributeType!: AttributeTypeDetailsGetDTO;

  get customValue() {
    return this.value.customValue;
  }

  set customValue(value) {
    this.$emit("input", { ...this.value, customValue: value });
  }

  get valueId() {
    return this.value.valueId;
  }

  set valueId(value) {
    this.$emit("input", { ...this.value, valueId: value });
  }

  get unitId() {
    return this.value.unitId;
  }

  set unitId(value) {
    this.$emit("input", { ...this.value, unitId: value });
  }

  mounted() {
    this.fetchAttributeType();
  }

  @Watch("typeId")
  fetchAttributeType(): void {
    this.fetched = false;
    if (this.typeId) {
      attributeTypesStore.getAttributeType(this.typeId).then((succeded) => {
        if (succeded) {
          this.attributeType = attributeTypesStore.selectedAttributeType!;

          let valueId = this.value.valueId;
          let unitId = this.value.unitId;
          let customValue = this.value.customValue;

          if (this.attributeType != null) {
            if (this.attributeType.usesDefinedValues) {
              valueId = this.attributeType.defaultValueId;
            } else {
              customValue = this.attributeType.defaultCustomValue;
            }
            if (this.attributeType.usesDefinedUnits) {
              unitId = this.attributeType.defaultUnitId;
            }
          }

          console.log(this.attributeType)

          this.$emit("input", { valueId, unitId, customValue });

          this.fetched = true;
        }
      });
    }
  }
}
</script>
