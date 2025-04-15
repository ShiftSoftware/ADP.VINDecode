Feature: VIN Extraction from Multiple Lines 2 (Same Binding)
  As a user
  I want to extract VINs from multiple lines
  
  Scenario: Extract VINs from multiple lines
    Given multiple lines
      | line1 | "Engine Size: 4.0 L" |
      | line2 | "VIN: 1FDKF37GXVEB34368 - 2025-03-26" |
      | line3 | "Brand: Some Brand" |
      | line4 | "Color: White" |
    When I extract VINs from the lines
    Then I should find a single VIN "1FDKF37GXVEB34368"