export interface AttributeTypeValueGetDTO {
  id: number;
  value: string;
}
export interface AttributeTypeValuePostDTO {
  value: string;
  attributeTypeId: number;
}

export interface AttributeTypeValuePatchDTO {
  id: number;
  value: string;
}
