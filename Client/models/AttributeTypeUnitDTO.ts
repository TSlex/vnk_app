export interface AttributeTypeUnitGetDTO {
  id: number;
  value: string;
}
export interface AttributeTypeUnitPostDTO {
  value: string;
  attributeTypeId: number;
}

export interface AttributeTypeUnitPatchDTO {
  id: number;
  value: string;
}
