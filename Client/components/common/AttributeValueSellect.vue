<template>
  <v-row v-if="fetched">
    <v-col>
      <CustomValueField
        :dataType="attributeType.dataType"
        v-model="customValue"
        :label="label"
        v-if="!attributeType.usesDefinedValues"
        class="ma-0"
      />
      <v-select
        v-else
        v-model="valueId"
        :items="attributeType.values"
        item-text="value"
        item-value="id"
        :label="label"
        class="ma-0"
      ></v-select>
    </v-col>
    <v-col v-if="attributeType.usesDefinedUnits">
      <v-select
        v-model="unitId"
        :items="attributeType.units"
        item-text="value"
        item-value="id"
        label="Ед. измерения"
        class="ma-0"
      ></v-select>
    </v-col>
  </v-row>
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

  @Prop({ default: "Значение" })
  label!: string;

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

  validateValue(id: number | null) {
    return (
      id != null &&
      _.includes(
        _.map(this.attributeType.values, (value) => value.id),
        id
      )
    );
  }

  validateUnit(id: number | null) {
    return (
      id != null &&
      _.includes(
        _.map(this.attributeType.units, (unit) => unit.id),
        id
      )
    );
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
            if (
              this.attributeType.usesDefinedValues &&
              !this.validateValue(valueId)
            ) {
              valueId = this.attributeType.defaultValueId;
            } else if (customValue.length == 0) {
              customValue = this.attributeType.defaultCustomValue;
            }
            if (
              this.attributeType.usesDefinedUnits &&
              !this.validateUnit(unitId)
            ) {
              unitId = this.attributeType.defaultUnitId;
            }
          }

          this.$emit("input", { valueId, unitId, customValue });

          this.fetched = true;
        }
      });
    }
  }
}
</script>
